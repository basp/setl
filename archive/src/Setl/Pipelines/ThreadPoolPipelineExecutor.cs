using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Pipelines;

public class ThreadPoolPipelineExecutor : AbstractPipelineExecutor
{
    public ThreadPoolPipelineExecutor(ILogger logger) : base(logger)
    {
    }

    protected override IEnumerable<Row> Decorate(
        IOperation operation, 
        IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}