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
