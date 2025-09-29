using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Setl.Operations;

/// <summary>
/// Provides a base class for operations.
/// </summary>
public abstract class AbstractOperation : IOperation
{
    private readonly ILogger logger;

    protected AbstractOperation(ILogger logger)
    {
        this.logger = logger;
    }
    
    private event Action<IOperation> onStartedProcessing =
        _ => { };

    private event Action<IOperation, Row> onRowProcessed =
        (_, _) => { };

    private event Action<IOperation> onFinishedProcessing =
        _ => { };
    
    /// <inheritdoc/>
    public virtual event Action<IOperation> StartedProcessing
    {
        add => this.onStartedProcessing += value;
        remove => this.onStartedProcessing -= value;
    }
    
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
    
    /// <summary>
    /// Provides the pipeline executor for this operation.
    /// </summary>
    /// <remarks>
    /// Normally this will be set by the <see cref="Prepare"/> method.
    /// </remarks>
    protected IPipelineExecutor PipelineExecutor { get; set; } =
        new UninitializedPipelineExecutor();
    
    /// <inheritdoc/>
    public virtual void Prepare(IPipelineExecutor pipelineExecutor)
    {
        this.onStartedProcessing(this);
        this.PipelineExecutor = pipelineExecutor;
    }
    
    /// <inheritdoc/>
    public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    protected virtual void Dispose(bool disposing)
    {
    }
    
    void IOperation.RaiseRowProcessed(Row row) =>
        this.onRowProcessed(this, row);
    
    void IOperation.RaiseFinishedProcessing() =>
        this.onFinishedProcessing(this);
}