using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public abstract class AbstractCommandOperation : AbstractOperation
{
    public AbstractCommandOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}