using Microsoft.Extensions.Logging;
using Setl.Enumerables;
using Setl.Pipelines;

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