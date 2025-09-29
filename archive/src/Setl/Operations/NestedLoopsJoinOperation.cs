using Microsoft.Extensions.Logging;
using Setl.Enumerables;

namespace Setl.Operations;

public abstract class NestedLoopsJoinOperation : AbstractJoinOperation
{
    private static readonly string IsEmptyRowMarker =
        Guid.NewGuid().ToString();

    protected NestedLoopsJoinOperation(ILogger logger)
        : base(logger)
    {
    }

    public NestedLoopsJoinOperation Left(IOperation op)
    {
        this.left.Register(op);
        return this;
    }

    public NestedLoopsJoinOperation Right(IOperation op)
    {
        this.right.Register(op);
        return this;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        this.PrepareForJoin();

        var matchedRightRows = new Dictionary<Row, object?>();
        var rightEnumerable = new CachingEnumerable<Row>(
            new EventRaisingEnumerator(
                this.right,
                this.right.Execute([])));
        var execute =
            this.left.Execute(this.isLeftRegistered ? [] : rows);
        foreach (Row leftRow in new EventRaisingEnumerator(this.left, execute))
        {
            var leftNeedsOuterJoin = true;
            foreach (Row rightRow in rightEnumerable)
            {
                if (this.MatchJoinCondition(leftRow, rightRow))
                {
                    leftNeedsOuterJoin = false;
                    matchedRightRows[rightRow] = null;
                    yield return this.MergeRows(leftRow, rightRow);
                }
            }

            if (leftNeedsOuterJoin)
            {
                var emptyRow = new Row
                {
                    [NestedLoopsJoinOperation.IsEmptyRowMarker] =
                        NestedLoopsJoinOperation.IsEmptyRowMarker
                };
                if (this.MatchJoinCondition(leftRow, emptyRow))
                {
                    yield return this.MergeRows(leftRow, emptyRow);
                }
                else
                {
                    this.LeftOrphanRow(leftRow);
                }
            }
        }

        foreach (Row rightRow in rightEnumerable)
        {
            if (matchedRightRows.ContainsKey(rightRow))
            {
                continue;
            }

            var emptyRow = new Row
            {
                [NestedLoopsJoinOperation.IsEmptyRowMarker] = NestedLoopsJoinOperation.IsEmptyRowMarker
            };
            if (this.MatchJoinCondition(emptyRow, rightRow))
            {
                yield return this.MergeRows(emptyRow, rightRow);
            }
            else
            {
                this.RightOrphanRow(rightRow);
            }
        }
    }

    protected abstract bool MatchJoinCondition(Row leftRow, Row rightRow);
}