using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class EtlProcess : EtlProcessBase<EtlProcess>, IDisposable
{
    private readonly IPipelineExecutor pipelineExecutor;
    
    protected EtlProcess(
        ILogger logger,
        IPipelineExecutor pipelineExecutor) 
        : base(logger) 
    {
        this.pipelineExecutor = pipelineExecutor;
    }

    protected static PartialProcessOperation CreatePartial()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        foreach (var op in this.operations)
        {
            op.Dispose();
        }
    }

    public void Execute()
    {
        this.Initialize();
        this.MergeLastOperations();
        this.RegisterToOperationEvents();
        this.LogTrace("Starting {Process}", this.Name);
        this.pipelineExecutor.Execute(
            this.Name, 
            this.operations, 
            this.Translate);
        this.PostProcessing();
    }

    public IEnumerable<Exception> GetAllErrors()
    {
        foreach (var err in this.Errors)
        {
            yield return err;
        }

        foreach (var err in this.pipelineExecutor.GetAllErrors())
        {
            yield return err;
        }

        foreach (var op in this.operations)
        {
            foreach (var err in op.GetAllErrors())
            {
                yield return err;
            }
        }
    }
    
    protected virtual IEnumerable<Row> Translate(IEnumerable<Row> rows) => 
        rows;
    
    protected abstract void Initialize();

    protected virtual void FinishedProcessing(IOperation op)
    {
    }

    protected virtual void RowProcessed(IOperation op, Row row)
    {
    }
    
    protected virtual void PostProcessing()
    {
    }

    private void RegisterToOperationEvents()
    {
        foreach (var op in this.operations)
        {
            op.RowProcessed += this.RowProcessed;
            op.FinishedProcessing += this.FinishedProcessing;
        }
    }
}