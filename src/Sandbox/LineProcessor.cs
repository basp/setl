using Sandbox.Entities;
using Sandbox.Parsing;
using Sandbox.Records;
using Sandbox.Support;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

internal class LineProcessor : ILineProcessor
{
    // Processing error handlers worden gebruikt om fouten op regelniveau
    // te behandelen. Er gaat in dit geval iets mis met de hele regel,
    // de structuur klopt bijvoorbeeld niet of hij kan uiteindelijk niet
    // geconverteerd worden naar een record.
    private readonly IProcessingErrorHandlers processingErrorHandlers;
    
    // Validation error handlers worden gebruikt om validatiefouten specifiek
    // en op kolomniveau te behandelen. In dit geval is de structuur van de
    // regel correct, maar is er met de inhoud van één of meer specifieke
    // velden iets mis.
    private readonly IValidationErrorHandlers validationErrorHandlers;
    
    // De visitor wordt gebruikt om de specifieke records te bezoeken en
    // daaruit een groepering (normaal op gemeenteniveau) te destilleren.
    // Tevens is dit object ook verantwoordelijk voor het valideren van de
    // structuur van het bericht.
    private readonly IRecordVisitor visitor;

    // Parsers worden gebruikt om de *data* sectie van elke regel op te splitsen
    // in lijst van (veldnaam, tekstwaarde) paren (i.e. een dictionary met tekst
    // keys en tekst waardes).
    private static readonly Dictionary<string, IFixedWidthParser> Parsers = new()
    {
        [KnownRecordTypes.Ber] = DataParsers.BerichtdataParser,
        [KnownRecordTypes.Gem] = DataParsers.GemeentedataParser,
        [KnownRecordTypes.Dtr] = DataParsers.DetaildataParser,
        [KnownRecordTypes.Tpg] = DataParsers.TellingdataParser,       
    };

    // Evaluators worden gebruikt om de waardes van tekstuele naam-waarde paren
    // om te zetten naar getypeerde waardes (een dictionary met tekst keys en
    // object waardes).
    private static readonly Dictionary<string, IDataEvaluator> Evaluators = new()
    {
        [KnownRecordTypes.Ber] = DataEvaluators.BerichtEvaluator,
        [KnownRecordTypes.Gem] = DataEvaluators.GemeenteEvaluator,
        [KnownRecordTypes.Dtr] = DataEvaluators.DetailEvaluator,
        [KnownRecordTypes.Tpg] = DataEvaluators.TellingdataEvaluator,
    };

    // Converters worden uiteindelijk gebruikt om een set van typed 
    // naam-waarde paren om te zetten naar een instantie van een strong
    // typed record. Ze zijn hier relatief *lui* gedefinieerd met kale
    // functies, maar we zouden ook kunnen overwegen om hier een interface
    // voor te definieren.
    private static readonly Dictionary<string, Func<Line, Row, IRecord>> Converters = new()
    {
        [KnownRecordTypes.Ber] = (line, row) =>
        {
            var record = row.ToObject<Berichtrecord>();
            return record with { Line = line };
        },
        [KnownRecordTypes.Gem] = (line, row) =>
        {
            var record = row.ToObject<Gemeenterecord>();
            return record with { Line = line };
        },       
        [KnownRecordTypes.Dtr] = (line, row) =>
        {
            var record = row.ToObject<Detailrecord>();
            return record with { Line = line };
        },       
        [KnownRecordTypes.Tpg] = (line, row) =>
        {
            var record = row.ToObject<Tellingenrecord>();
            return record with { Line = line };
        },       
    };
    
    public LineProcessor(
        IProcessingErrorHandlers processingErrorHandlers,
        IValidationErrorHandlers validationErrorHandlers,
        IRecordVisitor visitor)
    {
        this.processingErrorHandlers = processingErrorHandlers;
        this.validationErrorHandlers = validationErrorHandlers;
        this.visitor = visitor;
    }
    
    public ProcessingReportSummary Process(IEnumerable<Line> lines)
    {
        // Validators zijn niet static gedefinieerd omdat ze afhankelijk zijn
        // van `validationErrorHandlers` en deze instantie wordt runtime
        // geinjecteerd via de constructor. Deze instanties zijn
        // verantwoordelijk voor het uitvoeren van de daadwerkelijke validaatie
        // afhankelijk van de regelcode.
        var validators = new Dictionary<string, DataValidator>
        {
            [KnownRecordTypes.Ber] = new BerichtValidator(this.validationErrorHandlers),
            [KnownRecordTypes.Gem] = new GemeenteValidator(this.validationErrorHandlers),
            [KnownRecordTypes.Dtr] = new DetailValidator(this.validationErrorHandlers),
            [KnownRecordTypes.Tpg] = new TellingenValidator(this.validationErrorHandlers),
        };

        // Het totaal aantal rijen (records) dat we te verwerken kregen.
        // Dit zou gelijk moeten zijn aan het regels in het ontvangen bestand.
        var totalNumberOfRecords = 0;
        
        // Het aantal records dat het daadwerkelijk door (het grootste deel)
        // van de verwerking geschopt heeft. Dit aantal moet altijd minder or
        // gelijk zijn aan het totaal aantal verwerkingsfouten.
        var numberOfProcessedRecords = 0;
        
        foreach (var line in lines)
        {
            totalNumberOfRecords += 1;
            
            if (!Parsers.TryGetValue(line.Code, out var parser))
            {
                // Geen parser geregistreerd voor de code behorende
                // bij deze regel. Kan zijn dat we een rare code uitlezen
                // of dat we vergeten zijn om de bijbehorende parser te
                // registreren.
                this.processingErrorHandlers.OnOnbekendeParser(line);
                continue;
            }
            
            if (!parser.TryParse(line.Data, out var data))
            {
                // Regel heeft niet de juiste structuur. De gebruikte parsers
                // zijn vrij coulant dus dit komt zelden voor.
                this.processingErrorHandlers.OnMislukteParse(line);
                continue;
            }

            if (!validators.TryGetValue(line.Code, out var validator))
            {
                // Onbekende regelcode of vergeten validatie te registreren.
                this.processingErrorHandlers.OnOnbekendeValidatie(line);
                continue;
            }

            if (!validator.Validate(line, data))
            {
                // Regel heeft juiste structuur, maar één of meer velden
                // bevatten ongeldige waarden.
                //
                // Deze fouten worden via `validationErrorHandlers` gemeld en
                // komen via die weg uiteindelijk in processing errors
                // terecht.
                continue;
            }
            
            if (!Evaluators.TryGetValue(line.Code, out var evaluator))
            {
                // Onbekende regelcode of vergeten evaluatie te registreren.
                this.processingErrorHandlers.OnOnbekendeEvaluatie(line);
                continue;
            }

            if (!evaluator.TryEvaluate(data, out var row))
            {
                // Evaluatie hier betekent het omzetten van de tekstuele
                // waarden naar objecten. Als hier een probleem optreedt, 
                // dan is er waarschijnlijk een mismatch tussen de validatie
                // en de evaluatie voor een bepaalde regelcode (i.e. de 
                // validatie is niet juist of niet strict genoeg).
                this.processingErrorHandlers.OnMislukteEvaluatie(line);
                continue;
            }

            if (!Converters.TryGetValue(line.Code, out var converter))
            {
                // Onbekende regelcode of vergeten conversie te registreren.
                this.processingErrorHandlers.OnOnbekendeConversie(line);
                continue;
            }

            try
            {
                // Beetje risky here, standaard converters gebruiken onder de
                // motorkap `ToObject<T>` van `Row` en deze is niet super
                // intelligent.
                var record = converter(line, row);
                
                // Visitor kan potentieel ook exceptions opleveren indien er
                // iets mis is in de structuur (volgorde van records) in het
                // ontvangen bestand.
                record.Accept(this.visitor);
                
                // Als we op dit punt aanbeland zijn dan zijn alle validaties,
                // evaluaties en conversies op regelniveau (recordniveau)
                // geslaagd. Er kan echter later nog steeds iets mis gaan als
                // bijvoorbeeld de volgorde van de records niet juist is of
                // als bepaalde sleutelrecords (`BER`, `GEM` en `TPG`) ontbreken.
                numberOfProcessedRecords += 1;
            }
            catch (InvalidOperationException ex)
            {
                // Niet gelukt om het ge-evalueerde resultaat om te zetten
                // in een instantie van een recordtype. Bijna altijd een
                // mismatch tussen de evaluaties voor een bepaald record en
                // de types van de properties op het recordtype.
                // 
                // Deze situatie komt bijvoorbeeld voor als in de evaluatie
                // een conversie naar `int` gedaan wordt, maar de bijbehorende
                // property op het record gedefinieerd is als `string`.
                // Of anders gezegd, in het algemeen als er een verschil is
                // tussen het type van de evaluatie voor een bepaald veld en
                // bijbehorende property op het record.
                this.processingErrorHandlers.OnMislukteConversie(line, ex);
            }
        }
        
        // Zou minder of gelijk moeten zijn aan het aantal fouten in de error
        // lijst (er kunnen meerdere fouten per regel optreden).
        var numberOfFailed = totalNumberOfRecords - numberOfProcessedRecords;
        var generalErrorMessage =
            numberOfFailed > 0
                ? $"Er zijn {numberOfFailed} fouten opgetreden. Zie details voor meer informatie."
                : string.Empty;

        // Het bijhouden van individuele verwerkingsfouten is aan de client
        // die deze klasse gebruikt (dit gebeurt via de geinjecteerde error
        // handlers). Wij geven hier alleen maar een overzicht terug.
        return new ProcessingReportSummary
        {
            TotalNumberOfRecords = totalNumberOfRecords,
            NumberOfProcessedRecords = numberOfProcessedRecords,
            NumberOfFailedRecords = numberOfFailed,
            GeneralErrorMessage = generalErrorMessage,
            IsValid = numberOfFailed == 0,
        };
    }
}