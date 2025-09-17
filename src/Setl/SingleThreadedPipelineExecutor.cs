using Microsoft.Extensions.Logging;

namespace Setl;

public class SingleThreadedPipelineExecutor : AbstractPipelineExecutor
{
    public SingleThreadedPipelineExecutor(ILoggerFactory loggerFactory) 
        : base(loggerFactory.CreateLogger<SingleThreadedPipelineExecutor>())
    {
    }

    protected override IEnumerable<Row> Decorate(
        IOperation op, 
        IEnumerable<Row> rows) =>
            new CachingEnumerable<Row>(
                new EventRaisingEnumerator(op, rows));
}