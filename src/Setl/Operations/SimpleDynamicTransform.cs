using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class SimpleDynamicTransform : AbstractOperation
{
    private readonly Func<dynamic, dynamic> transform;
    
    public SimpleDynamicTransform(
        Func<dynamic, dynamic> transform,
        ILogger logger) : base(logger)
    {
        this.transform = transform;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (dynamic row in rows)
        {
            yield return this.transform(row);
        }
    }
}