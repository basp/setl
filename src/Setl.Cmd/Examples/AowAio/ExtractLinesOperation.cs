using Microsoft.Extensions.Logging;
using Setl.Cmd.AowAio;
using Setl.Operations;

namespace Setl.Cmd.Examples.AowAio;

public class ExtractLinesOperation : AbstractOperation
{
    public ExtractLinesOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
        var index = 0;
        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var source = reader.ReadLine();
            var line = new Line
            {
                Index = index,
                Source = source,
            };
            
            yield return Row.FromObject(line);
            index++;
        }
    }
}