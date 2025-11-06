using System.Text.Json;
using Sandbox.Parsing;
using Sandbox.Support;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

public static class Program
{
    public static void Main(params string[] args)
    {
        const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_goed.dat.txt";
        // const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_kapot.dat.txt";
        
        var preprocessor = new LineParser
        {
            OnInvalidLine = (i, s) =>
                Console.WriteLine($"invalid line ({i}): [{s}]"),
        };

        var parsers = new Dictionary<string, IFixedWidthParser>
        {
            ["BER"] = DataParsers.BerichtdataParser,
            ["GEM"] = DataParsers.GemeentedataParser,
            ["DTR"] = DataParsers.DetaildataParser,
            ["TPG"] = DataParsers.TellingdataParser,       
        };

        var validationErrorHandlers = new AccumulatingValidationErrorHandlers();
        var validators = new Dictionary<string, DataValidator>
        {
            ["BER"] = new BerichtValidator(validationErrorHandlers),
            ["GEM"] = new GemeenteValidator(validationErrorHandlers),
            ["DTR"] = new DetailValidator(validationErrorHandlers),
            ["TPG"] = new TellingenValidator(validationErrorHandlers),
        };

        var evaluators = new Dictionary<string, IDataEvaluator>
        {
            ["BER"] = DataEvaluators.BerichtEvaluator,
            ["GEM"] = DataEvaluators.GemeenteEvaluator,
            ["DTR"] = DataEvaluators.DetailEvaluator,
            ["TPG"] = DataEvaluators.TellingdataEvaluator,
        };

        var converters = new Dictionary<string, Func<Row, object>>
        {
            ["BER"] = RowConverters.ToBerichtrecord,
            ["GEM"] = RowConverters.ToGemeenterecord,
            ["DTR"] = RowConverters.ToDetailrecord,
            ["TPG"] = RowConverters.ToTellingenrecord,         
        };
        
        var lines = preprocessor.Parse(File.ReadLines(path));
        foreach (var line in lines)
        {
            if (!parsers.TryGetValue(line.Code, out var parser))
            {
                Console.WriteLine($"No parser for [{line.Code}]");
                continue;
            }
            
            if (!validators.TryGetValue(line.Code, out var validator))
            {
                Console.WriteLine($"No validator for [{line.Code}]");
                continue;
            }

            if (!parser.TryParse(line.Data, out var data))
            {
                Console.WriteLine($"Cannot parse [{line.Source}]");
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

            if (!converters.TryGetValue(
                    line.Code, 
                    out var converter))
            {
                Console.WriteLine($"No converter for [{line.Code}]");
                continue;
            }
            
            var record = converter(row);
            PrintRow(Row.FromObject(record));
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
}
