using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class AbstractJoinOperation : AbstractOperation
{
    protected readonly PartialProcessOperation left;
    protected readonly PartialProcessOperation right;
    protected bool isLeftRegistered = false;
    
    protected AbstractJoinOperation(ILogger logger) : base(logger)
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

    public override void PrepareForExecution(IPipelineExecutor pipelineExecutor)
    {
        this.left.PrepareForExecution(pipelineExecutor);
        this.right.PrepareForExecution(pipelineExecutor);
    }

    public override IEnumerable<Exception> GetAllErrors()
    {
        foreach (var error in this.left.GetAllErrors())
        {
            yield return error;
        }
        
        foreach (var error in this.right.GetAllErrors())
        {
            yield return error;
        }

        foreach (var error in this.Errors)
        {
            yield return error;
        }
    }

    public override void Dispose()
    {
        this.left.Dispose();
        this.right.Dispose();
    }
    
    protected abstract Row MergeRows(Row leftRow, Row rightRow);
}