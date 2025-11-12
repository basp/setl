using Sandbox.Entities;
using Sandbox.Parsing;
using Sandbox.Records;
using Sandbox.Support;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

internal class LineProcessor : ILineProcessor
{
    private readonly IProcessingErrorHandlers processingErrorHandlers;
    private readonly IValidationErrorHandlers validationErrorHandlers;
    private readonly IRecordVisitor visitor;
    
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
        var parsers = new Dictionary<string, IFixedWidthParser>
        {
            [KnownRecordTypes.Ber] = DataParsers.BerichtdataParser,
            [KnownRecordTypes.Gem] = DataParsers.GemeentedataParser,
            [KnownRecordTypes.Dtr] = DataParsers.DetaildataParser,
            [KnownRecordTypes.Tpg] = DataParsers.TellingdataParser,       
        };

        var validators = new Dictionary<string, DataValidator>
        {
            [KnownRecordTypes.Ber] = new BerichtValidator(validationErrorHandlers),
            [KnownRecordTypes.Gem] = new GemeenteValidator(validationErrorHandlers),
            [KnownRecordTypes.Dtr] = new DetailValidator(validationErrorHandlers),
            [KnownRecordTypes.Tpg] = new TellingenValidator(validationErrorHandlers),
        };

        var evaluators = new Dictionary<string, IDataEvaluator>
        {
            [KnownRecordTypes.Ber] = DataEvaluators.BerichtEvaluator,
            [KnownRecordTypes.Gem] = DataEvaluators.GemeenteEvaluator,
            [KnownRecordTypes.Dtr] = DataEvaluators.DetailEvaluator,
            [KnownRecordTypes.Tpg] = DataEvaluators.TellingdataEvaluator,
        };

        var converters = new Dictionary<string, Func<Line, Row, IRecord>>
        {
            [KnownRecordTypes.Ber] = (line, row) =>
            {
                var record = row.ToObject<Berichtrecord>();
                record.Line = line;
                return record;
            },
            [KnownRecordTypes.Gem] = (line, row) =>
            {
                var record = row.ToObject<Gemeenterecord>();
                record.Line = line;
                return record;
            },       
            [KnownRecordTypes.Dtr] = (line, row) =>
            {
                var record = row.ToObject<Detailrecord>();
                record.Line = line;
                return record;
            },       
            [KnownRecordTypes.Tpg] = (line, row) =>
            {
                var record = row.ToObject<Tellingenrecord>();
                record.Line = line;
                return record;
            },       
        };
        
        var totalNumberOfRecords = 0;
        var totalNumberProcessed = 0;
        
        foreach (var line in lines)
        {
            totalNumberOfRecords += 1;
            
            if (!parsers.TryGetValue(line.Code, out var parser))
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
                // Rare code of vergeten validatie te registreren.
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
            
            if (!evaluators.TryGetValue(line.Code, out var evaluator))
            {
                // Rare code of vergeten evaluatie te registreren.
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

            if (!converters.TryGetValue(line.Code, out var converter))
            {
                // Rare code of vergeten conversie te registreren.
                this.processingErrorHandlers.OnOnbekendeConversie(line);
                continue;
            }

            try
            {
                var record = converter(line, row);
                record.Accept(visitor);
                
                // Als we op dit punt aanbeland zijn dan zijn alle validaties,
                // evaluaties en conversies op regelniveau (recordniveau)
                // geslaagd. Er kan echter later nog steeds iets mis gaan als
                // bijvoorbeeld de volgorde van de records niet juist is of
                // als bepaalde sleutelrecords (`BER`, `GEM` en `TPG`) ontbreken.
                totalNumberProcessed += 1;
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
                // tussen de evaluatie voor een bepaald veld en bijbehorende
                // property op het record.
                this.processingErrorHandlers.OnMislukteConversie(line, ex);
            }
        }
        
        // Zou minder of gelijk moeten zijn aan het totaal aantal fouten.
        var numberOfFailed = totalNumberOfRecords - totalNumberProcessed;
        var generalErrorMessage =
            numberOfFailed > 0
                ? $"Er zijn {numberOfFailed} fouten opgetreden. Zie details voor meer informatie."
                : string.Empty;
            
        return new ProcessingReportSummary
        {
            TotalNumberOfRecords = totalNumberOfRecords,
            NumberOfProcessedRecords = totalNumberProcessed,
            NumberOfFailedRecords = numberOfFailed,
            GeneralErrorMessage = generalErrorMessage,
            IsValid = numberOfFailed == 0,
        };
    }
}