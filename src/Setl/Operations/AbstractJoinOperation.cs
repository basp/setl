using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public abstract class AbstractJoinOperation : AbstractOperation
{
    protected readonly PartialProcessOperation left;
    protected readonly PartialProcessOperation right;
    
    protected bool isLeftRegistered = false;

    private bool disposed = false;
    
    protected AbstractJoinOperation(ILogger logger)
        : base(logger)
    {
        this.left = new PartialProcessOperation(logger);
        this.right = new PartialProcessOperation(logger);
    }

    protected virtual void Initialize()
    {
    }

    protected virtual void RightOrphanRow(Row row)
    {
    }
    
    protected virtual void LeftOrphanRow(Row row)
    {
    }

    protected void PrepareForJoin()
    {
        this.Initialize();
        ArgumentNullException.ThrowIfNull(this.left, nameof(this.left));
        ArgumentNullException.ThrowIfNull(this.right, nameof(this.right));
    }

    public override void Prepare(IPipelineExecutor pipelineExecutor)
    {
        this.left.Prepare(pipelineExecutor);
        this.right.Prepare(pipelineExecutor);
    }

    public override IEnumerable<Exception> GetErrors()
    {
        var leftErrors = this.left.GetErrors();
        var rightErrors = this.right.GetErrors();
        var ownErrors = this.logger.GetErrors();
        return leftErrors.Concat(rightErrors).Concat(ownErrors);
    }
        
    protected abstract Row MergeRows(Row leftRow, Row rightRow);
    
    protected override void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }
        
        if (disposing)
        {
            this.left.Dispose();
            this.right.Dispose();
        }
        
        this.disposed = true;
        base.Dispose(disposing);
    }
}