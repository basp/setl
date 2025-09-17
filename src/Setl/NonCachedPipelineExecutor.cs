using Microsoft.Extensions.Logging;

namespace Setl;

public class NonCachedPipelineExecutor : AbstractPipelineExecutor
{
    public NonCachedPipelineExecutor(ILoggerFactory loggerFactory) 
        : base(loggerFactory.CreateLogger<NonCachedPipelineExecutor>())
    {
    }

    protected override IEnumerable<Row> Decorate(
        IOperation op, 
        IEnumerable<Row> rows)
    {
        foreach (Row row in new EventRaisingEnumerator(op, rows))
        {
            yield return row;
        }
    }
}