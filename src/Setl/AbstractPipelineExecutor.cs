using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class AbstractPipelineExecutor 
    : LoggerAdapter, IPipelineExecutor
{
    protected AbstractPipelineExecutor(ILogger logger) 
        : base(logger)
    {
    }

    public event Action<IPipelineExecutor>? Starting;

    public event Action<IPipelineExecutor>? Completing;
    
    public virtual bool HasErrors { get; private set; }

    public virtual void Execute(
        string name, 
        ICollection<IOperation> pipeline, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        try
        {
            var enumerablePipeline = this.PipelineToEnumerable(
                pipeline, 
                new List<Row>(), 
                translate);

            try
            {
                this.OnStarting();
                var start = DateTime.Now;
                this.ExecutePipeline(enumerablePipeline);
                this.OnCompleting();
                var duration = DateTime.Now - start;
                this.LogTrace(
                    "Completed {Pipeline} in {Duration}",
                    name,
                    duration);
            }
            catch (Exception ex)
            {
                this.LogError(
                    ex, 
                    "Failed to execute {Pipeline}", 
                    name);
            }
        }
        catch (Exception ex)
        {
            this.LogError(
                ex,
                "Failed to create {Pipeline}",
                name);
        }

        this.DisposeOperations(pipeline);
    }
    
    public virtual IEnumerable<Row> PipelineToEnumerable(
        IEnumerable<IOperation> pipeline, 
        IEnumerable<Row> rows, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        foreach (var op in pipeline)
        {
            op.PrepareForExecution(this);
            var enumerable = op.Execute(rows);
            enumerable = translate(enumerable);
            rows = this.Decorate(op, enumerable);
        }

        return rows;
    }

    public virtual IEnumerable<Exception> GetAllErrors() => this.Errors;

    protected virtual void ExecutePipeline(IEnumerable<Row> pipeline)
    {
        using var enumerator = pipeline.GetEnumerator();
        try
        {
            // Execute pipeline operations as a command.
            while (enumerator.MoveNext())
            {
            }
        }
        catch (Exception ex)
        {
            this.LogError(
                ex,
                "Failed to execute operation {Operation}",
                enumerator.Current);
        }
    }

    protected abstract IEnumerable<Row> Decorate(
        IOperation operation,
        IEnumerable<Row> rows);

    protected virtual void DisposeOperations(ICollection<IOperation> operations)
    {
        foreach (var op in operations)
        {
            try
            {
                op.Dispose();
            }
            catch (Exception ex)
            {
                this.LogError(
                    ex, 
                    "Failed to dispose {Operation}", 
                    op.Name);
            }
        }
    }
    
    protected virtual void OnStarting()
    {
        this.Starting?.Invoke(this);
    }

    protected virtual void OnCompleting()
    {
        this.Completing?.Invoke(this);
    }
}