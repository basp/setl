using Microsoft.Extensions.Logging;

namespace Setl.Operations;

public class PartialProcessOperation
    : EtlProcessBase<PartialProcessOperation>, IOperation
{
    private IPipelineExecutor executor =
        new UninitializedPipelineExecutor();

    private bool disposed;

    internal PartialProcessOperation(ILogger logger)
        : base(logger)
    {
    }
    
    public PartialProcessOperation(ILoggerFactory factory)
        : base(factory.CreateLogger<PartialProcessOperation>())
    {
    }

    public event Action<IOperation, Row> RowProcessed
    {
        add
        {
            foreach (var op in this.operations)
            {
                op.RowProcessed += value;           
            }
        }
        remove
        {
            foreach (var op in this.operations)
            {
                op.RowProcessed -= value;           
            }
        }
    }

    public event Action<IOperation>? FinishedProcessing
    {
        add
        {
            foreach (var op in this.operations)
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

    public OperationStatistics Statistics { get; } = new();
    
    public void Prepare(IPipelineExecutor pipelineExecutor)
    {
        this.executor = pipelineExecutor;
        foreach (var op in this.operations)
        {
            op.Prepare(this.executor);
        }
    }

    public IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        this.MergeLastOperations();
        return this.executor.ToPipeline(
            this.operations,
            rows,
            enumerable => enumerable);
    }

    public IEnumerable<Exception> GetErrors() => 
        this.operations.SelectMany(x => x.GetErrors());

    public virtual void RaiseRowProcessed(Row row)
    {
    }

    public virtual void RaiseFinishedProcessing()
    {
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);   
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }
        
        if (disposing)
        {
            foreach (var op in this.operations)
            {
                op.Dispose();
            }
            
        }
    
        this.disposed = true;   
    }
}