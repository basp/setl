namespace Setl.Operations;

public abstract class AbstractOperation : IOperation
{
    private event Action<IOperation> onStartedProcessing =
        _ => { };

    private event Action<IOperation, Row> onRowProcessed =
        (_, _) => { };

    private event Action<IOperation> onFinishedProcessing =
        _ => { };
    
    public virtual event Action<IOperation> StartedProcessing
    {
        add => this.onStartedProcessing += value;
        remove => this.onStartedProcessing -= value;
    }
    
    public virtual event Action<IOperation, Row> RowProcessed
    {
        add => this.onRowProcessed += value;
        remove => this.onRowProcessed -= value;
    }
    
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
    
    public virtual void Prepare(IPipelineExecutor pipelineExecutor)
    {
        this.onStartedProcessing(this);
        this.PipelineExecutor = pipelineExecutor;
    }
    
    public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
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