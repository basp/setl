using Microsoft.Extensions.Logging;
using Setl.Enumerables;
using Setl.Operations;

namespace Setl.Pipelines;

public class SingleThreadedNonCachedPipelineExecutor
    : AbstractPipelineExecutor
{
    public SingleThreadedNonCachedPipelineExecutor(ILogger logger) 
        : base(logger)
    {
    }

    protected override IEnumerable<Row> Decorate(
        IOperation operation, 
        IEnumerable<Row> rows)
    {
        foreach (Row row in new EventRaisingEnumerator(operation, rows))
        {
            yield return row;
        }
    }
}