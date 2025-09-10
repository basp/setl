using Microsoft.Extensions.Logging;

namespace Setl;

public class NamedOperation : AbstractOperation
{
    private readonly IOperation op;
    
    public NamedOperation(
        string name,
        IOperation op,
        ILogger logger) 
        : base(logger)
    {
        this.Name = name;
        this.op = op;
    }

    public override string Name { get; }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows) =>
        this.op.Execute(rows);
}