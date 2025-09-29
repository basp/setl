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
        ICollection<IOperation> pipeline,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        try
        {
            var enumerable = this.ToEnumerable(pipeline, [], translate);
            try
            {
                this.RaiseStaring();
                var start = DateTime.Now;
                this.ExecutePipeline(enumerable);
                this.RaiseFinishing();
                var duration = DateTime.Now - start;
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
    }
    
    public virtual IEnumerable<Row> ToEnumerable(
        IEnumerable<IOperation> pipeline,
        IEnumerable<Row> rows,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate)
    {
        foreach (var op in pipeline)
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
    
    protected abstract IEnumerable<Row> Decorate(
        IOperation op, 
        IEnumerable<Row> rows);
}