using Microsoft.Extensions.Logging;
using Setl.Cmd.Avd;
using Setl.Operations;
using Parser = Setl.Cmd.AowAio.Parser;

namespace Setl.Cmd.Examples.AowAio;

public class ParseRecordsOperation : AbstractOperation
{
    public ParseRecordsOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            if (Parser.TryParse((string)row["Source"], out var parsed))
            {
                yield return row;
            }
        }
    }
}