using Microsoft.Extensions.Logging;

namespace Setl;

public class PartialProcessOperation
    : EtlProcessBase<PartialProcessOperation>, IOperation
{
    private IPipelineExecutor? pipelineExecutor;

    public PartialProcessOperation(ILogger logger) 
        : base(logger)
    {
    }
    
    public OperationStatistics Statistics { get; } = new();

    public event Action<IOperation, Row>? OnRowProcessed
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.OnRowProcessed += value;
            }
        }
        remove
        {
            foreach(var op in this.operations)
            {
                op.OnRowProcessed -= value;
            }
        }
    }
    public event Action<IOperation>? OnFinishedProcessing
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.OnFinishedProcessing += value;
            }
        }
        remove
        {
            foreach (var op in this.operations)
            {
                op.OnFinishedProcessing -= value;           
            }
        }
    }

    public void PrepareForExecution(IPipelineExecutor executor)
    {
        this.pipelineExecutor = executor;
        foreach (var op in this.operations)
        {
            op.PrepareForExecution(pipelineExecutor);
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