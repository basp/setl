using Microsoft.Extensions.Logging;
using Setl.Enumerables;

namespace Setl.Operations;

public class BranchingOperation : AbstractBranchingOperation
{
    public BranchingOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        var copied = new CachingEnumerable<Row>(rows);
        foreach (var op in this.Operations)
        {
            var cloned = copied.Select(x => x.Clone());
            var enumerable = op.Execute(cloned);
            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
            {
            }
        }

        yield break;
    }
}