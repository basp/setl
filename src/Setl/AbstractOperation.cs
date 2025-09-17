using Microsoft.Extensions.Logging;

namespace Setl;

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

    public virtual string Name => this.GetType().Name;

    protected IPipelineExecutor PipelineExecutor { get; set; } =
        new ThrowingPipelineExecutor();
    
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
    
    private class ThrowingPipelineExecutor : IPipelineExecutor
    {
        public void Execute(
            string name, 
            ICollection<IOperation> pipeline, 
            Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
                throw CreatePipelineExecutorException();

        public IEnumerable<Row> ToEnumerable(
            IEnumerable<IOperation> pipeline, 
            IEnumerable<Row> rows, 
            Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
                throw CreatePipelineExecutorException();

        private static Exception CreatePipelineExecutorException()
        {
            const string msg = 
                "The pipeline executor is not correctly initialized.";
            return new InvalidOperationException(msg);
        }
    }
}