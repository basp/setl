using Microsoft.Extensions.Logging;

namespace Setl;

public class SingleThreadedNonCachedPipelineExecutor
    : AbstractPipelineExecutor
{
    public SingleThreadedNonCachedPipelineExecutor(ILoggerFactory loggerFactory) 
        : base(loggerFactory.CreateLogger<SingleThreadedNonCachedPipelineExecutor>())
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