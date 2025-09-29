using Microsoft.Extensions.Logging;

namespace Setl.Operations;

/// <summary>
/// Provides a base class for operations.
/// </summary>
public abstract class AbstractOperation : IOperation
{
    internal readonly ErrorCollectingLoggerAdapter logger;
    
    protected AbstractOperation(ILogger logger)
    {
        this.logger = new ErrorCollectingLoggerAdapter(logger);
    }
    
    private event Action<IOperation, Row> onRowProcessed =
        (_, _) => { };

    private event Action<IOperation> onFinishedProcessing =
        _ => { };
    
    /// <inheritdoc/>
    public virtual event Action<IOperation, Row> RowProcessed
    {
        add => this.onRowProcessed += value;
        remove => this.onRowProcessed -= value;
    }
    
    /// <inheritdoc/>
    public virtual event Action<IOperation> FinishedProcessing
    {
        add => this.onFinishedProcessing += value;
        remove => this.onFinishedProcessing -= value;
    }

    /// <inheritdoc/>
    public virtual string Name => this.GetType().Name;

    /// <inheritdoc/>
    public OperationStatistics Statistics { get; } = new();
    
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    internal IPipelineExecutor PipelineExecutor { get; private set; } =
        new UninitializedPipelineExecutor();

    /// <inheritdoc/>
    // ReSharper disable once ParameterHidesMember
    public virtual void Prepare(IPipelineExecutor pipelineExecutor)
    {
        this.PipelineExecutor = pipelineExecutor;
        this.Statistics.MarkStarted();
    }
    
    /// <inheritdoc/>
    public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
    /// <inheritdoc/>
    public virtual IEnumerable<Exception> GetErrors() => 
        this.logger.GetErrors();
    
    /// <inheritdoc/>
    public override string ToString() =>
        $"{this.Name} ({this.Statistics.Processed} rows processed in {this.Statistics.Duration})";


    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    void IOperation.RaiseRowProcessed(Row row)
    {
        this.Statistics.MarkRowProcessed();
        this.onRowProcessed(this, row);
    }

    void IOperation.RaiseFinishedProcessing()
    {
        this.Statistics.MarkFinished();
        this.onFinishedProcessing(this);
    }
}