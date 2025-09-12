using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public abstract class AbstractBranchingOperation : AbstractOperation
{
    protected AbstractBranchingOperation(ILogger logger) : base(logger)
    {
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected List<IOperation> Operations { get; } = [];

    public override event Action<IOperation, Row>? RowProcessed
    {
        add
        {
            foreach (var op in this.Operations)
            {
                op.RowProcessed += value;   
            }
            
            base.RowProcessed += value;       
        }
        remove
        {
            foreach(var op in this.Operations)
            {
                op.RowProcessed -= value;
            }
            
            base.RowProcessed -= value;
        }
    }
    
    public override event Action<IOperation>? FinishedProcessing
    {
        add
        {
            foreach (var op in this.Operations)
            {
                op.FinishedProcessing += value;   
            }
            
            base.FinishedProcessing += value;
        }
        remove
        {
            foreach(var op in this.Operations)
            {
                op.FinishedProcessing -= value;
            }
            
            base.FinishedProcessing -= value;
        }
    }
    
    public AbstractBranchingOperation Add(IOperation op)
    {
        this.Operations.Add(op);
        return this;
    }

    public override void PrepareForExecution(IPipelineExecutor pipelineExecutor)
    {
        base.PrepareForExecution(pipelineExecutor);
        foreach (var op in this.Operations)
        {
            op.PrepareForExecution(pipelineExecutor);
        }
    }
}