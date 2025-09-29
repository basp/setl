using Microsoft.Extensions.Logging;

namespace Setl;

/// <summary>
/// Provides a bass class for ETL processes.
/// </summary>
public abstract class EtlProcess
    : EtlProcessBase<EtlProcess>, IDisposable
{
    private readonly IPipelineExecutor pipelineExecutor;
    
    private bool disposed;

    /// <summary>
    /// Creates a new <see cref="EtlProcess"/> instance.
    /// </summary>
    /// <param name="pipelineExecutor">The pipeline executor to use.</param>
    /// <param name="logger">The logger to use.</param> 
    protected EtlProcess(
        IPipelineExecutor pipelineExecutor,
        ILogger logger)
        : base(logger)
    {
        this.pipelineExecutor = pipelineExecutor;
    }

    /// <summary>
    /// Executes the ETL process by initializing operations, setting up event
    /// handling, and invoking the pipeline executor to process all operations.
    /// Finalizes the process with post-processing steps.
    /// </summary>
    public void Execute()
    {
        this.Initialize();
        this.MergeLastOperations();
        this.WireOperationEvents();
        this.logger.LogTrace("Starting to execute {Process}", this.Name);
        this.pipelineExecutor.Execute(
            this.Name,
            this.operations,
            this.Translate);
        this.PostProcessing();
    }

    public IEnumerable<Exception> GetErrors()
    {
        throw new NotImplementedException();
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }
    
    /// <summary>
    /// Initializes the process.
    /// </summary>
    /// <remarks>
    /// This can be used for any setup tasks and is also typically where
    /// operations are registered.
    /// </remarks>
    protected abstract void Initialize();

    protected virtual IEnumerable<Row> Translate(IEnumerable<Row> rows) =>
        rows;

    protected virtual void OnFinishedProcessing(IOperation op)
    {
        this.logger.LogTrace(
            "Finished {Operation}: {Statistics}", 
            op.Name,
            op.Statistics);
    }
    
    protected virtual void OnRowProcessed(IOperation op, Row row)
    {
        if (op.Statistics.Processed % 1000 == 0)
        {
            this.logger.LogInformation(
                "Processed {Processed} rows in {Operation}",
                op.Statistics.Processed,
                op.Name);
        }
        else
        {
            this.logger.LogDebug(
                "Processed {Processed} rows in {Operation}",
                op.Name,
                op.Statistics);
        }
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