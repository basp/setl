using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Avd.Operations;

public class WriteAvdRecordsOperation : AbstractOperation
{
    public WriteAvdRecordsOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            Console.WriteLine(row.ToJson());
        }

        yield break;
    }
}