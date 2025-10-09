using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Avd.Operations;

public class ExtractAvdRecordsOperation : AbstractOperation
{
    private readonly StreamReader reader;
    private readonly Parser parser;

    public ExtractAvdRecordsOperation(
        StreamReader reader,
        Parser parser,
        ILogger logger)
        : base(logger)
    {
        this.reader = reader;
        this.parser = parser;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
        this.parser
            .Parse(this.reader)
            .Where(x => x.IsLineType(LineType.Record))
            .Where(x => !x.IsFlagged());
}