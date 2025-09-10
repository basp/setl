using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class AbstractOperation : LoggerAdapter, IOperation
{
    private event Action<IOperation, Row>? onRowProcessed;
    
    private event Action<IOperation>? onFinishedProcessing;
    
    protected AbstractOperation(ILogger logger) : base(logger)
    {
    }
    
    protected IPipelineExecutor? PipelineExecutor { get; set; }
    
    public virtual string Name => this.GetType().Name;
    
    public bool UseTransaction { get; set; } = true;

    public OperationStatistics Statistics { get; } = new();

    public virtual event Action<IOperation, Row>? OnRowProcessed
    {
        add => this.onRowProcessed += value;
        remove => this.onRowProcessed -= value;
    }

    public virtual event Action<IOperation>? OnFinishedProcessing
    {
        add => this.onFinishedProcessing += value;
        remove => this.onFinishedProcessing -= value;
    }

    public virtual void PrepareForExecution(IPipelineExecutor pipelineExecutor)
    {
        this.PipelineExecutor = pipelineExecutor;
        this.Statistics.MarkStarted();
    }

    public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);

    void IOperation.RaiseRowProcessed(Row row)
    {
        this.Statistics.MarkRowProcessed();
        this.onRowProcessed?.Invoke(this, row);  
    }
    
    void IOperation.RaiseFinishedProcessing()
    {
        this.Statistics.MarkFinished();
        this.onFinishedProcessing?.Invoke(this);   
    } 

    public virtual IEnumerable<Exception> GetAllErrors() => this.Errors;
    
    public virtual void Dispose()
    {
    }
}