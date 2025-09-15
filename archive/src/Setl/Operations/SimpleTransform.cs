using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class SimpleTransform : AbstractOperation
{
    private readonly Func<Row, Row> transform;
    
    public SimpleTransform(
        Func<Row, Row> transform,
        ILogger logger) 
        : base(logger)
    {
        this.transform = transform;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
        rows.Select(this.transform);
}