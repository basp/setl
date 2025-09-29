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
        this.isLeftRegistered = true;
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

    /// <summary>
    /// Returns <c>true</c> if the two rows should be joined.
    /// </summary>
    /// <param name="leftRow">The left side of the join.</param>
    /// <param name="rightRow">The right side of the join.</param>
    /// <returns>
    /// <c>true</c> if the rows should be joined, otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// When implementing this method, it's usually preferable to use
    /// <c>Equals(obj, obj)</c> to check for equality unless you have a specific
    /// reason not to.
    /// </remarks>
    protected abstract bool MatchJoinCondition(Row leftRow, Row rightRow);
}