using Sandbox.Parsing;
using Sandbox.Text;
using Sandbox.Validation;

namespace Sandbox;

public static class Program
{
    public static void Main(params string[] args)
    {
        const string path = @"d:/temp/SVB/SVBWWB65PLUS00002_3.txt";
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

        // This variation will cache all the errors in memory
        // so they can be queried later.
        var validationErrorHandlers = 
            new AccumulatingValidationErrorHandlers();

        var validators = new Dictionary<string, DataValidator>
        {
            ["BER"] = new BerichtValidator(validationErrorHandlers),
            ["GEM"] = new GemeenteValidator(validationErrorHandlers),
            ["DTR"] = new DetailValidator(validationErrorHandlers),
            ["TPG"] = new TellingenValidator(validationErrorHandlers),
        };
        
        var lines = preprocessor.Parse(File.ReadLines(path));
        foreach (var line in lines)
        {
            if (!dataParsers.TryGetValue(line.Code, out var parser))
            {
                // No parser for this code.
                continue;
            }
            
            if (!validators.TryGetValue(line.Code, out var validator))
            {
                // No validator for this code.
                continue;
            }

            if (!parser.TryParse(line.Data, out var data))
            {
                // Unable to parse, according to the parser.
                continue;
            }

            if (!validator.Validate(line, data))
            {
                // Text values do not conform to specification.
                continue;
            }
            
            var header = $"-- [{line.Code}] ".PadRight(20, '-');
            Console.WriteLine(header);
            foreach (var field in data)
            {
                Console.WriteLine($"{field.Key}: [{field.Value}]");
            }
        }

        foreach (var error in validationErrorHandlers.Errors)
        {
            var message = $"{error.Message} (regel {error.Context.Line.Index})";
            Console.WriteLine(message);
        }
    }
}
