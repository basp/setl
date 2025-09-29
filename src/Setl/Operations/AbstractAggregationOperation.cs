using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class AbstractAggregationOperation : AbstractOperation
{
    public AbstractAggregationOperation(ILogger logger) 
        : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}