using Microsoft.Extensions.Logging;

namespace Setl;

public class SingleThreadedPipelineExecutor : AbstractPipelineExecutor
{
    public SingleThreadedPipelineExecutor(ILogger logger) : base(logger)
    {
    }

    protected override IEnumerable<Row> Decorate(
        IOperation operation, 
        IEnumerable<Row> rows)
    {
        return new CachingEnumerable<Row>(
            new EventRaisingEnumerator(operation, rows));
    }
}