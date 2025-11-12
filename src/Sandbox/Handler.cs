using System.Text.Json;
using Sandbox.Parsing;
using Sandbox.Records;
using Sandbox.Validation;

namespace Sandbox;

public class Handler
{
    public void Execute(string path)
    {
        var preprocessor = new LineParser
        {
            OnInvalidLine = (i, s) =>
                Console.WriteLine($"invalid line ({i}): [{s}]"),
        };
        
        var errors = new List<ProcessingErrorData>();
        var processingErrorHandlers = new ProcessingErrorHandlers(errors);
        var validationErrorHandlers = new ValidationErrorHandlers(errors);
        var visitor = new AccumulatingRecordVisitor();
        
        var processor = new LineProcessor(
            processingErrorHandlers, 
            validationErrorHandlers, 
            visitor);

        var lines = preprocessor.Parse(File.ReadLines(path));
        var report = processor.Process(lines);

        Console.WriteLine("Total number of records  : " + report.TotalNumberOfRecords);
        Console.WriteLine("Total number of processed: " + report.NumberOfProcessedRecords);
        Console.WriteLine("Totaal foutieve records  : " + report.NumberOfFailedRecords);
        
        // TODO: Work page aanmaken met verwerkingsverslag
        // TODO: ProcessingReportDataReceived event publiceren
        
        if (!report.IsValid)
        {
            // Er zijn eén of meer verwerkingsfouten opgetreden.
            // Specifieke informatie over de fouten is in de details
            // van het verslag terug te vinden.
            
            // TODO: ProcesMonitoringHelper.AddFailureProcessStepAction
            // TODO: this.logger.LogError
            
            PrintErrors(errors);
            
            // Hierna hoeven we geen verdere acties meer te ondernemen.
            return;
        }

        try
        {
            var (bericht, tellingen) = visitor.GetResults();
            
            // Op dit moment weten we dat niet alleen de individuele records
            // juist zijn, maar ook dat de structuur van het bestand voldoet
            // aan onze verwachtingen.
            PrintResults(bericht, tellingen);
        }
        catch (InvalidOperationException)
        {
            // Er is hoogstwaarschijnlijk iets mis met de algemene
            // structuur (i.e. volgorde van de records, ontbrekende records) in
            // het ontvangen bestand. In dit geval hebben we geen andere
            // processing errors (anders waren we hier niet eens gekomen)
            // dus kunnen we alleen maar een general error message genereren
            // met behulp van de exception message.
            //
            // Deze situatie zou eigenlijk niet voor moeten komen, dit
            // betekent dat er iets mis gegaan is bij de partij waarvan we het
            // bericht ontvangen hebben *of* dat onze verwachtingen niet
            // kloppen.

            // TODO: ProcessMonitoringHelper.AddFailureProcessStepAction
            // TODO: this.logger.LogError
        }
    }

    private static void PrintErrors(List<ProcessingErrorData> errors)
    {
        foreach (var error in errors)
        {
            Console.WriteLine(error.ErrorMessage);
        }
    }

    private static void PrintResults(Berichtrecord bericht, List<AccumulatedResult> tellingen)
    {
        var opts = new JsonSerializerOptions
        {
            WriteIndented = true,
            IndentSize = 2,
        };

        Console.WriteLine(JsonSerializer.Serialize(bericht, opts));
        Console.WriteLine(JsonSerializer.Serialize(tellingen, opts));
    }

    private record AccumulatedResult(Gemeenterecord Gemeente)
    {
        public List<Detailrecord> Details { get; set; } = [];
        
        public Tellingenrecord? Tellingen { get; set; }
    }

    private class ProcessingErrorHandlers : IProcessingErrorHandlers
    {
        private readonly List<ProcessingErrorData> errors;
        
        public ProcessingErrorHandlers(List<ProcessingErrorData> errors)
        {
            this.errors = errors; 
        }
        
        public void OnOnbekendeParser(Line line)
        {
            this.AddError(Errors.OnbekendeParser, line);
        }

        public void OnMislukteParse(Line line)
        {
            this.AddError(Errors.OngeldigeRegel, line);
        }

        public void OnOnbekendeValidatie(Line line)
        {
            this.AddError(Errors.OnbekendeValidatie, line);
        }

        public void OnOnbekendeEvaluatie(Line line)
        {
            this.AddError(Errors.OnbekendeEvaluatie, line);
        }

        public void OnMislukteEvaluatie(Line line)
        {
            this.AddError(Errors.MislukteEvaluatie, line);
        }

        public void OnOnbekendeConversie(Line line)
        {
            this.AddError(Errors.OnbekendeConversie, line);
        }

        public void OnMislukteConversie(Line line, Exception ex)
        {
            this.AddError(Errors.MislukteConversie, line, ex);
        }

        private void AddError(Error error, Line line)
        {
            var errorMessage = $"{error.Message} (regel {line.Index})";
            var errorData = new ProcessingErrorData(
                error.ErrorCode, 
                string.Empty, 
                errorMessage);
            this.errors.Add(errorData);
        }

        private void AddError(Error error, Line line, Exception ex)
        {
            var errorMessage = $"{error.Message} (regel {line.Index}): {ex.Message}";
            var errorData = new ProcessingErrorData(
                error.ErrorCode, 
                string.Empty, 
                errorMessage);
            this.errors.Add(errorData);
        }
    }
    
    private class ValidationErrorHandlers : IValidationErrorHandlers
    {
        private readonly List<ProcessingErrorData> errors;

        public ValidationErrorHandlers(List<ProcessingErrorData> errors)
        {
            this.errors = errors;
        }
        
        public void OnFunctieVersieNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.FunctieVersieNietNumeriek, context);
        }

        public void OnCodeSectorLeverancierNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.CodeSectorLeverancierNietNumeriek, context);
        }

        public void OnCodeSectorAanvragerNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.CodeSectorAanvragerNietNumeriek, context);
        }

        public void OnDatumAanmaakBerichtNietGeldig(ValidationErrorContext context)
        {
            this.AddError(Errors.DatumAanmaakBerichtNietGeldig, context);
        }

        public void OnGemeentecodeNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.GemeentecodeNietNumeriek, context);
        }

        public void OnVerwerkingsjaarNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.VerwerkingsjaarNietNumeriek, context);
        }

        public void OnVerwerkingsmaandNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.VerwerkingsmaandNietNumeriek, context);
        }

        public void OnBsnHpNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.BsnHpNietNumeriek, context);
        }

        public void OnBsnHpNiet11Proef(ValidationErrorContext context)
        {
            this.AddError(Errors.BsnHpNiet11Proef, context);
        }

        public void OnGeboortedatumHpNietGeldig(ValidationErrorContext context)
        {
            this.AddError(Errors.GeboortedatumHpNietGeldig, context);
        }

        public void OnWwbBedragHpNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.WwbBedragHpNietNumeriek, context);
        }

        public void OnPostcodeNumeriekNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.PostcodeNumeriekNietNumeriek, context);
        }

        public void OnHuisnummerNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.HuisnummerNietNumeriek, context);
        }

        public void OnBsnPNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.BsnPartnerNietNumeriek, context);
        }

        public void OnBsnPNiet11Proef(ValidationErrorContext context)
        {
            this.AddError(Errors.BsnPartnerNiet11Proef, context);
        }

        public void OnGeboortedatumPNietGeldig(ValidationErrorContext context)
        {
            this.AddError(Errors.GeboortedatumPartnerNietGeldig, context);
        }

        public void OnWwbBedragPNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.WwbBedragPartnerNietNumeriek, context);
        }

        public void OnIngangsdatumRechtNietGeldig(ValidationErrorContext context)
        {
            this.AddError(Errors.IngangsdatumRechtNietGeldig, context);
        }

        public void OnEinddatumRechtNietGeldig(ValidationErrorContext context)
        {
            this.AddError(Errors.EinddatumRechtNietGeldig, context);
        }

        public void OnTotaalAantalGerechtigdenNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.TotaalAantalGerechtigdenNietNumeriek, context);
        }

        public void OnTotaalAantalHuishoudensNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.TotaalAantalHuishoudensNietNumeriek, context);
        }

        public void OnTotaalWwbBedragNietNumeriek(ValidationErrorContext context)
        {
            this.AddError(Errors.TotaalWwbBedragNietNumeriek, context);
        }
        
        private void AddError(Error error, ValidationErrorContext context)
        {
            var errorMessage = $"{error.Message} (regel {context.Line.Index})";
            var errorData = new ProcessingErrorData(
                error.ErrorCode, 
                string.Empty, 
                errorMessage);
            this.errors.Add(errorData);
        }
    }
    
    private class AccumulatingRecordVisitor : IRecordVisitor
    {
        private readonly Stack<AccumulatedResult> current = new();

        private readonly List<Berichtrecord> berichten = [];
        private readonly List<AccumulatedResult> results = [];
        
        public (Berichtrecord, List<AccumulatedResult>) GetResults()
        {
            if (this.berichten.Count == 0)
            {
                const string message = "Bestand ontvangen zonder `BER` regel.";
                throw new InvalidOperationException(message);
            }

            if (this.berichten.Count > 1)
            {
                // Meerdere berichtregels gevonden.
                // Waarschuwing?
            }

            var bericht = this.berichten.Last();
            return (bericht, this.results); 
        }
        
        public void VisitBericht(Berichtrecord record)
        {
            this.berichten.Add(record);
        }

        public void VisitGemeente(Gemeenterecord record)
        {
            if (this.current.Count > 0)
            {
                // Hier hebben we een `GEM` regel maar voor de
                // vorige `GEM` zijn we nog geen `TPG` tegengekomen.
                var gemeentecode = this.current.Peek().Gemeente;
                var message = $"Gemeenterecord zonder `TPG` voor gemeente {gemeentecode.Gemeentecode}";
                throw new InvalidOperationException(message);
            }
            
            var acc = new AccumulatedResult(record);
            this.current.Push(acc);
        }

        public void VisitDetail(Detailrecord record)
        {
            if (this.current.Count == 0)
            {
                // Hier hebben we een `DTR` regel maar we zijn geen
                // bijbehorende `GEM` regel tegengekomen.
                var message = $"Detailrecord zonder `GEM` regel";
                throw new InvalidOperationException(message);
            }
            
            var acc = this.current.Peek();
            acc.Details.Add(record);
        }

        public void VisitTellingen(Tellingenrecord record)
        {
            if (this.current.Count == 0)
            {
                // Hier hebben we een `TPG` regel maar we hebben
                // geen bijbehorende `GEM` regel gezien.
                var message = $"Tellingenrecord zonder `GEM` regel";
                throw new InvalidOperationException(message);
            }
            
            var acc = this.current.Pop();
            if (acc.Gemeente.Gemeentecode != record.Gemeentecode)
            {
                // We hebben een `GEM` maar nu komen we een `TPG` tegen
                // met een andere gemeentecode.
                var message = $"Gemeentecode verschilt tussen `GEM` en `TPG` regels";
                throw new InvalidOperationException(message);           
            }
            
            acc.Tellingen = record;
            this.results.Add(acc);
        }
    }
}