using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class JoinOperation : AbstractJoinOperation
{
    private JoinType joinType;
    
    private string[] leftColumns = [];
    private string[] rightColumns = [];
    
    private readonly Dictionary<Row, object?> rightRowsMatched = 
        new();
    private readonly Dictionary<CompositeKey, List<Row>> rightRowsByJoinKey = 
        new();
    
    protected JoinOperation(ILogger logger) 
        : base(logger)
    {
    }

    public override event Action<IOperation, Row>? OnRowProcessed
    {
        add
        {
            var ops = new[] { this.left, this.right };
            foreach (var op in ops)
            {
                op.OnRowProcessed += value;
            }
            
            base.OnRowProcessed += value;
        }
        remove
        {
            var ops = new [] { this.left, this.right };
            foreach (var op in ops)
            {
                op.OnRowProcessed -= value;           
            }
            
            base.OnRowProcessed -= value;
        }
    }

    public override event Action<IOperation>? OnFinishedProcessing
    {
        add
        {
            var ops = new [] { this.left, this.right };
            foreach (var op in ops)
            {
                op.OnFinishedProcessing += value;
            }
            
            base.OnFinishedProcessing += value;
        }
        remove
        {
            var ops = new [] { this.left, this.right };
            foreach (var op in ops)
            {
                op.OnFinishedProcessing -= value;           
            }
            
            base.OnFinishedProcessing -= value;
        }
    }

    public JoinOperation Right(IOperation value)
    {
        this.right.Register(value);
        return this;
    }

    public JoinOperation Left(IOperation value)
    {
        this.left.Register(value);
        this.isLeftRegistered = true;
        return this;
    }
    
    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        this.PrepareForJoin();
        this.SetupJoinConditions();
        
        ArgumentNullException.ThrowIfNull(
            this.leftColumns, 
            nameof(this.leftColumns));;
        ArgumentNullException.ThrowIfNull(
            this.rightColumns,
            nameof(this.rightColumns));

        var rightEnumerable = this.GetRightEnumerable();
        var execute = this.left.Execute(
            this.isLeftRegistered ? [] : rows);

        foreach (Row leftRow in new EventRaisingEnumerator(this.left, execute))
        {
            var key = leftRow.CreateKey();
            if (this.rightRowsByJoinKey.TryGetValue(key, out var rightRows))
            {
                foreach (var rightRow in rightRows)
                {
                    this.rightRowsMatched[rightRow] = null;
                    yield return this.MergeRows(leftRow, rightRow);
                }
            }
            else if ((this.joinType & JoinType.Left) != 0)
            {
                var emptyRow = new Row();
                yield return this.MergeRows(leftRow, emptyRow);
            }
            else
            {
                this.LeftOrphanRow(leftRow);
            }
        }

        foreach (Row rightRow in rightEnumerable)
        {
            if (this.rightRowsMatched.ContainsKey(rightRow))
            {
                continue;
            }

            var emptyRow = new Row();
            if ((this.joinType & JoinType.Right) != 0)
            {
                yield return this.MergeRows(emptyRow, rightRow);
            }
            else
            {
                this.RightOrphanRow(rightRow);
            }
        }
    }

    protected abstract void SetupJoinConditions();

    protected JoinBuilder InnerJoin =>
        new(this, JoinType.Inner);
    
    protected JoinBuilder LeftJoin =>
        new(this, JoinType.Left);
    
    protected JoinBuilder RightJoin =>
        new(this, JoinType.Right);
    
    protected JoinBuilder FullJoin =>
        new(this, JoinType.Full);
    
    private CachingEnumerable<Row> GetRightEnumerable()
    {
        var rightEnumerable = new CachingEnumerable<Row>(
            new EventRaisingEnumerator(
                this.right,
                this.right.Execute([])));

        foreach (Row row in rightEnumerable)
        {
            var key = row.CreateKey();
            if (!this.rightRowsByJoinKey.TryGetValue(key, out var rowsForKey))
            {
                this.rightRowsByJoinKey[key] = rowsForKey = [];
            }
            
            rowsForKey.Add(row);
        }
        
        return rightEnumerable;
    }
    
    public class JoinBuilder
    {
        private readonly JoinOperation parent;

        public JoinBuilder(JoinOperation parent, JoinType joinType)
        {
            this.parent = parent;
            this.parent.joinType = joinType;
        }

        public JoinBuilder Left(params string[] columns)
        {
            this.parent.leftColumns = columns;
            return this;
        }

        public JoinBuilder Right(params string[] columns)
        {
            this.parent.rightColumns = columns;
            return this;
        }
    }
}