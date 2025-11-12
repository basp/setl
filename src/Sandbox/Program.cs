using System.Text.Json;
using Sandbox.Parsing;
using Sandbox.Records;
using Sandbox.Support;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

public static class Program
{
    public static void Main(params string[] args)
    {
        // const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_goed.dat.txt";
        const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_kapot.dat.txt";
        
        var preprocessor = new LineParser
        {
            OnInvalidLine = (i, s) =>
                Console.WriteLine($"invalid line ({i}): [{s}]"),
        };

        var parsers = new Dictionary<string, IFixedWidthParser>
        {
            [KnownRecordType.Ber] = DataParsers.BerichtdataParser,
            [KnownRecordType.Gem] = DataParsers.GemeentedataParser,
            [KnownRecordType.Dtr] = DataParsers.DetaildataParser,
            [KnownRecordType.Tpg] = DataParsers.TellingdataParser,       
        };

        var validationErrorHandlers = new AccumulatingValidationErrorHandlers();
        var validators = new Dictionary<string, DataValidator>
        {
            [KnownRecordType.Ber] = new BerichtValidator(validationErrorHandlers),
            [KnownRecordType.Gem] = new GemeenteValidator(validationErrorHandlers),
            [KnownRecordType.Dtr] = new DetailValidator(validationErrorHandlers),
            [KnownRecordType.Tpg] = new TellingenValidator(validationErrorHandlers),
        };

        var evaluators = new Dictionary<string, IDataEvaluator>
        {
            [KnownRecordType.Ber] = DataEvaluators.BerichtEvaluator,
            [KnownRecordType.Gem] = DataEvaluators.GemeenteEvaluator,
            [KnownRecordType.Dtr] = DataEvaluators.DetailEvaluator,
            [KnownRecordType.Tpg] = DataEvaluators.TellingdataEvaluator,
        };

        var converters = new Dictionary<string, Func<Row, IRecord>>
        {
            [KnownRecordType.Ber] = row => row.ToObject<Berichtrecord>(),
            [KnownRecordType.Gem] = row => row.ToObject<Gemeenterecord>(),       
            [KnownRecordType.Dtr] = row => row.ToObject<Detailrecord>(),       
            [KnownRecordType.Tpg] = row => row.ToObject<Tellingenrecord>(),       
        };
        
        // var visitor = new TestRecordVisitor();
        var visitor = new AccumulatingRecordVisitor();       
        
        var lines = preprocessor.Parse(File.ReadLines(path));
        foreach (var line in lines)
        {
            if (!parsers.TryGetValue(line.Code, out var parser))
            {
                Console.WriteLine($"No parser for [{line.Code}]");
                continue;
            }
            
            if (!parser.TryParse(line.Data, out var data))
            {
                Console.WriteLine($"Cannot parse [{line.Source}]");
                continue;
            }

            if (!validators.TryGetValue(line.Code, out var validator))
            {
                Console.WriteLine($"No validator for [{line.Code}]");
                continue;
            }

            if (!validator.Validate(line, data))
            {
                Console.WriteLine($"Invalid data [{line.Source}]");
                continue;
            }
            
            if (!evaluators.TryGetValue(line.Code, out var evaluator))
            {
                Console.WriteLine($"No evaluator for [{line.Code}]");
                continue;
            }

            if (!evaluator.TryEvaluate(data, out var row))
            {
                Console.WriteLine($"Cannot evaluate [{line.Source}]");
                continue;
            }

            if (!converters.TryGetValue(line.Code, out var converter))
            {
                Console.WriteLine($"No converter for [{line.Code}]");
                continue;
            }
            
            var record = converter(row);
            record.Accept(visitor);

            // PrintRow(Row.FromObject(record));
        }

        try
        {
            var (bericht, tellingen) =
                visitor.GetResults();

            var opts = new JsonSerializerOptions
            {
                WriteIndented = true,
                IndentSize = 2,
            };

            Console.WriteLine(JsonSerializer.Serialize(bericht, opts));
            Console.WriteLine(JsonSerializer.Serialize(tellingen, opts));
        }
        catch (InvalidOperationException)
        {
            Console.WriteLine("Failed to get results.");
        }
            
        foreach (var error in validationErrorHandlers.Errors)
        {
            PrintError(error);
        }
    }

    private static void PrintError(ValidationError error)
    {
        var message = $"{error.Message} (regel {error.Context.Line.Index})";
        Console.WriteLine(message);
    }
    
    private static void PrintRow(Row row)
    
    {
        var opts = new JsonSerializerOptions
        {
            IndentSize = 2,
            WriteIndented = true,
        };
            
        Console.WriteLine(row.ToJson(opts));
    }

    private record AccumulatedResult(Gemeenterecord Gemeente)
    {
        public List<Detailrecord> Details { get; set; } = [];
        
        public Tellingenrecord? Tellingen { get; set; }
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
                // Bericht ingelezen zonder `BER` regel.
                throw new InvalidOperationException();
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
                throw new InvalidOperationException();
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
                throw new InvalidOperationException();
            }
            
            var acc = this.current.Peek();
            
            // Detail records hebben zelf geen gemeentecode in brondata dus
            // voegen we deze voor het gemak hier maar toe. Beetje hacky.
            // Strict gezien ook niet noodzakelijk, maar wel net zo netjes.
            record.Gemeentecode = acc.Gemeente.Gemeentecode.ToString();
            
            acc.Details.Add(record);
        }

        public void VisitTellingen(Tellingenrecord record)
        {
            if (this.current.Count == 0)
            {
                // Hier hebben we een `TPG` regel maar we hebben
                // geen (potentieel) bijbehorende `GEM` regel gezien.
                throw new InvalidOperationException();
            }
            
            var acc = this.current.Pop();
            if (acc.Gemeente.Gemeentecode != record.Gemeentecode)
            {
                // We hebben een `GEM` maar nu komen we een `TPG` tegen
                // met een andere gemeentecode.
                throw new InvalidOperationException();           
            }
            
            acc.Tellingen = record;
            this.results.Add(acc);
        }
    }

    private class TestRecordVisitor : IRecordVisitor
    {
        public void VisitBericht(Berichtrecord record)
        {
            Console.WriteLine("BER");
        }

        public void VisitGemeente(Gemeenterecord record)
        {
            Console.WriteLine("GEM");
        }

        public void VisitDetail(Detailrecord record)
        {
            Console.WriteLine("DTR");
        }

        public void VisitTellingen(Tellingenrecord record)
        {
            Console.WriteLine("TPG");
        }
    }
}
