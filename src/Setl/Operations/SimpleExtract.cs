using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class SimpleExtract : AbstractOperation
{
    private readonly Func<IEnumerable<Row>> extract;
    
    public SimpleExtract(
        Func<IEnumerable<Row>> extract,
        ILogger logger) 
        : base(logger)
    {
        this.extract = extract;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
        this.extract();
}