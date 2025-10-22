using System.Text.Json;
using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Examples.AowAio;

public class WriteLines : AbstractOperation
{
    public WriteLines(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            var opts = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            
            Console.WriteLine(row.ToJson(opts));
            yield return row;
        }
    }
}