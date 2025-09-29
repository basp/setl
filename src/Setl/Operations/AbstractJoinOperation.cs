using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class AbstractJoinOperation : AbstractOperation
{
    protected AbstractJoinOperation(ILogger logger)
        : base(logger)
    {
    }
    
    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}