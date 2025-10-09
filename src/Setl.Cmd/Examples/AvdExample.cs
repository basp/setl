using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;
using Setl.Cmd.Avd;

namespace Setl.Cmd.Examples;

public static class AvdExample
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        const string path = @"D:\temp\BD\avd_corrupt_3.csv";
        var reader = new StreamReader(path);
        var executor = new SingleThreadedPipelineExecutor(loggerFactory);
        var logger = loggerFactory.CreateLogger<AvdExampleEtlProcess>();
        
        AvdExampleEtlProcess.OnParseError = (i, m, s) =>
        {
            logger.LogWarning("{Message} (index {Index}): {Source}", m, i, s);
        };
        
        var process = new AvdExampleEtlProcess(
            reader,
            executor,
            loggerFactory.CreateLogger<AvdExampleEtlProcess>());
        
        process.Execute();
    }
}

// //
// // public static class AvdExample
// // {
// //     private static List<(int, string, string)> ParseErrors = [];
// //     
// //     public static void Run(ILoggerFactory _)
// //     {
// //         var parser = new Avd.Parser()
// //         {
// //             OnHeaderParseError = (i, s) =>
// //             {
// //                 var error = (i, "Invalid header", s);
// //                 AvdExample.ParseErrors.Add(error);
// //             },
// //             OnRecordParseError = (i, s) =>
// //             {
// //                 var error = (i, "Invalid record", s);
// //                 AvdExample.ParseErrors.Add(error);
// //             },
// //         };
// //         
// //         const string path = @"D:\temp\BD\avd_corrupt.csv";
// //         using var reader = new StreamReader(path);
// //         var extracted = 
// //             parser.Parse(reader);
// //         var records = 
// //             extracted.Where(x => x.IsLineType(LineType.Record));
// //         
// //         Console.WriteLine("== Extracted ==");
// //         foreach (var record in records)
// //         {
// //             Console.WriteLine(record.ToJson());
// //         }
// //
// //         Console.WriteLine();
// //         
// //         Console.WriteLine("== Errors ==");
// //         foreach (var (i, m, s) in AvdExample.ParseErrors)
// //         {
// //             Console.WriteLine($"{m} (index {i}): {s}");
// //         }
// //     }
// }