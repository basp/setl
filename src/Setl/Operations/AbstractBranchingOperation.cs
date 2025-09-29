using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class AbstractBranchingOperation : AbstractOperation
{
    protected AbstractBranchingOperation(ILogger logger)
        : base(logger)
    {
    }
    
    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}