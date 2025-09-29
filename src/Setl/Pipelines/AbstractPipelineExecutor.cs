using Microsoft.Extensions.Logging;

namespace Setl.Pipelines;

public abstract class AbstractPipelineExecutor : IPipelineExecutor
{
    public event Action<IPipelineExecutor> Starting = _ => { };
    
    public event Action<IPipelineExecutor> Finishing = _ => { };

    private readonly ILogger logger;

    protected AbstractPipelineExecutor(ILogger logger)
    {
        this.logger = logger;
    }
    
    public virtual void Execute(
        string name,
        ICollection<IOperation> operations,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        try
        {
            var pipeline = this.ToPipeline(
                operations, 
                [], 
                translate);
            
            try
            {
                this.RaiseStaring();
                var start = DateTime.Now;
                this.ExecutePipeline(pipeline);
                this.RaiseFinishing();
                var duration = DateTime.Now - start;
                this.logger.LogTrace(
                    "Completed process {Process} in {Duration}",
                    name,
                    duration);
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex, 
                    "Failed to execute {Pipeline}", 
                    name);
            }
        }
        catch(Exception ex)
        {
            this.logger.LogError(
                ex, 
                "Failed to create {Pipeline}", 
                name);
        }
        
        this.DisposeOperations(operations);
    }
    
    /// <summary>
    /// Connects the registered operations into a single enumerable of
    /// <see cref="Row"/> objects. 
    /// </summary>
    /// <param name="operations">The operations for the pipeline.</param>
    /// <param name="rows">Any input rows.</param>
    /// <param name="translate">
    /// A function to translate rows between operations.
    /// </param>
    /// <returns></returns>
    public virtual IEnumerable<Row> ToPipeline(
        IEnumerable<IOperation> operations,
        IEnumerable<Row> rows,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        foreach (var op in operations)
        {
            op.Prepare(this);
            var enumerable = op.Execute(rows);
            enumerable = translate(enumerable);
            rows = this.Decorate(op, enumerable);
        }

        return rows;
    }

    protected virtual void ExecutePipeline(IEnumerable<Row> pipeline)
    {
        using var enumerator = pipeline.GetEnumerator();
        try
        {
            // Drain the pipeline (i.e. execute operations)
            while (enumerator.MoveNext())
            {
            }
        }
        catch (Exception ex)
        {
            this.logger.LogError(
                ex,
                "Failed to execute operation {Operation}",
                enumerator.Current);
        }
    }

    protected virtual void RaiseStaring() =>
        this.Starting(this);
    
    protected virtual void RaiseFinishing() =>
        this.Finishing(this);

    protected void DisposeOperations(ICollection<IOperation> operations)
    {
        foreach (var op in operations)
        {
            try
            {
                op.Dispose();
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    ex,
                    "Failed to dispose operation {Operation}",
                    op.Name);
            }
        }
    }
    
    protected abstract IEnumerable<Row> Decorate(
        IOperation op, 
        IEnumerable<Row> rows);
}