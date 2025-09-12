using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class SortMergeJoinOperation : AbstractOperation
{
    public SortMergeJoinOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}