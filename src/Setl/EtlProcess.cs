namespace Setl;

public abstract class EtlProcess
    : EtlProcessBase<EtlProcess>, IDisposable
{
    private readonly IPipelineExecutor pipelineExecutor;
    
    private bool disposed;
    
    protected EtlProcess(IPipelineExecutor pipelineExecutor)
    {
        this.pipelineExecutor = pipelineExecutor;
    }

    public void Execute()
    {
        this.Initialize();
        this.MergeLastOperations();
        this.WireOperationEvents();
        this.pipelineExecutor.Execute(
            this.Name,
            this.operations,
            this.Translate);
        this.PostProcessing();
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }
    

    protected abstract void Initialize();

    protected virtual IEnumerable<Row> Translate(IEnumerable<Row> rows) =>
        rows;

    protected virtual void OnFinishedProcessing(IOperation op)
    {
    }
    
    protected virtual void OnRowProcessed(IOperation op, Row row)
    {
    }

    protected virtual void PostProcessing()
    {
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

    private void WireOperationEvents()
    {
        foreach(var op in this.operations)
        {
            op.RowProcessed += this.OnRowProcessed;
            op.FinishedProcessing += this.OnFinishedProcessing;
        }
    }
}