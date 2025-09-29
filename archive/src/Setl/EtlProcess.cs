using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl;

public abstract class EtlProcess : EtlProcessBase<EtlProcess>, IDisposable
{
    private readonly ILogger logger;
    private readonly IPipelineExecutor pipelineExecutor;
    private bool disposed;
    
    protected EtlProcess(
        ILogger logger,
        IPipelineExecutor pipelineExecutor) 
        : base(logger) 
    {
        this.logger = logger;
        this.pipelineExecutor = pipelineExecutor;
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);   
    }

    public void Execute()
    {
        this.Initialize();
        this.MergeLastOperations();
        this.RegisterToOperationEvents();
        this.LogTrace("Starting {Process}", this.Name);
        this.pipelineExecutor.Execute(
            this.Name, 
            this.operations, 
            this.Translate);
        this.PostProcessing();
    }

    public IEnumerable<Exception> GetAllErrors()
    {
        foreach (var err in this.Errors)
        {
            yield return err;
        }

        foreach (var err in this.pipelineExecutor.GetAllErrors())
        {
            yield return err;
        }

        foreach (var op in this.operations)
        {
            foreach (var err in op.GetAllErrors())
            {
                yield return err;
            }
        }
    }
    
    protected virtual IEnumerable<Row> Translate(IEnumerable<Row> rows) => 
        rows;
    
    protected abstract void Initialize();

    protected virtual void FinishedProcessing(IOperation op)
    {
    }

    protected virtual void RowProcessed(IOperation op, Row row)
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

    private void RegisterToOperationEvents()
    {
        foreach (var op in this.operations)
        {
            op.RowProcessed += this.RowProcessed;
            op.FinishedProcessing += this.FinishedProcessing;
        }
    }
}