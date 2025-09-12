using Microsoft.Extensions.Logging;
using Setl.Enumerables;
using Setl.Operations;

namespace Setl.Pipelines;

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