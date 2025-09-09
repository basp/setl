using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class AbstractOperation : LoggerAdapter, IOperation
{
    protected AbstractOperation(ILogger logger) : base(logger)
    {
    }
    
    protected IPipelineExecutor? PipelineExecutor { get; set; }
    
    public virtual string Name => this.GetType().Name;
    
    public bool UseTransaction { get; set; } = true;

    public OperationStatistics Statistics { get; } = new();

    public event Action<IOperation, Row>? OnRowProcessed;
    
    public event Action<IOperation>? OnFinishedProcessing;
    
    public virtual void PrepareForExecution(IPipelineExecutor pipelineExecutor)
    {
        this.PipelineExecutor = pipelineExecutor;
        this.Statistics.MarkStarted();
    }

    public abstract IEnumerable<Row> Execute(IEnumerable<Row> rows);

    void IOperation.RaiseRowProcessed(Row row)
    {
        this.Statistics.MarkRowProcessed();
        this.OnRowProcessed?.Invoke(this, row);  
    }
    
    void IOperation.RaiseFinishedProcessing()
    {
        this.Statistics.MarkFinished();
        this.OnFinishedProcessing?.Invoke(this);   
    } 

    public IEnumerable<Exception> GetAllErrors() => this.Errors;
    
    public virtual void Dispose()
    {
    }
}