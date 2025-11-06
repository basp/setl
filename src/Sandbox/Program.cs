using Sandbox.Parsing;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

public static class Program
{
    public static void Main(params string[] args)
    {
        const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_goed.dat.txt";
        var preprocessor = new LineParser
        {
            OnInvalidLine = (i, s) =>
                Console.WriteLine($"invalid line ({i}): [{s}]"),
        };

        var dataParsers = new Dictionary<string, IFixedWidthParser>
        {
            ["BER"] = DataParsers.BerichtdataParser,
            ["GEM"] = DataParsers.GemeentedataParser,
            ["DTR"] = DataParsers.DetaildataParser,
            ["TPG"] = DataParsers.TellingdataParser,       
        };

        var validationErrorHandlers = new AccumulatingValidationErrorHandlers();
        // var validationErrorHandlers = new ThrowingValidationErrorHandlers();

        var validators = new Dictionary<string, DataValidator>
        {
            ["BER"] = new BerichtValidator(validationErrorHandlers),
            ["GEM"] = new GemeenteValidator(validationErrorHandlers),
            ["DTR"] = new DetailValidator(validationErrorHandlers),
            ["TPG"] = new TellingenValidator(validationErrorHandlers),
        };

        var evaluators = new Dictionary<string, IDataEvaluator>
        {
            ["BER"] = Evaluators.BerichtEvaluator,
            ["GEM"] = Evaluators.GemeenteEvaluator,
            ["DTR"] = Evaluators.DetailEvaluator,
            ["TPG"] = Evaluators.TellingdataEvaluator,
        };
        
        var lines = preprocessor.Parse(File.ReadLines(path));
        foreach (var line in lines)
        {
            if (!dataParsers.TryGetValue(line.Code, out var parser))
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
            
            var row = evaluator.Evaluate(data);
            Console.WriteLine(row.ToJson());
        }

        foreach (var error in validationErrorHandlers.Errors)
        {
            var message = $"{error.Message} (regel {error.Context.Line.Index})";
            Console.WriteLine(message);
        }
    }
}
