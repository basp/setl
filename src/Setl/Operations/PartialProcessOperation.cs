using Microsoft.Extensions.Logging;

namespace Setl.Operations;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class PartialProcessOperation
    : EtlProcessBase<PartialProcessOperation>, IOperation
{
    private IPipelineExecutor? pipelineExecutor;

    public PartialProcessOperation(ILogger logger) 
        : base(logger)
    {
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
    public OperationStatistics Statistics { get; } = new();

    public event Action<IOperation, Row>? RowProcessed
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.RowProcessed += value;
            }
        }
        remove
        {
            foreach(var op in this.operations)
            {
                op.RowProcessed -= value;
            }
        }
    }
    public event Action<IOperation>? FinishedProcessing
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.FinishedProcessing += value;
            }
        }
        remove
        {
            foreach (var op in this.operations)
            {
                op.FinishedProcessing -= value;           
            }
        }
    }

    public void PrepareForExecution(IPipelineExecutor executor)
    {
        this.pipelineExecutor = executor;
        foreach (var op in this.operations)
        {
            op.PrepareForExecution(this.pipelineExecutor);
        }
        
        this.Statistics.MarkStarted();
    }

    public IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        this.MergeLastOperations();
        return this.pipelineExecutor!.PipelineToEnumerable(
            this.operations,
            rows,
            enumerable => enumerable);
    }

    public void RaiseRowProcessed(Row row)
    {
        this.Statistics.MarkRowProcessed();
    }

    public void RaiseFinishedProcessing()
    {
        this.Statistics.MarkFinished();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);   
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }
        
        foreach(var op in this.operations)
        {
            op.Dispose();
        }
    }

    public IEnumerable<Exception> GetAllErrors()
    {
        return this
            .operations
            .SelectMany(op => op.GetAllErrors());
    }
}