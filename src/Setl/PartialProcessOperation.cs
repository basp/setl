using Microsoft.Extensions.Logging;

namespace Setl;

public class PartialProcessOperation
    : EtlProcessBase<PartialProcessOperation>, IOperation
{
    protected PartialProcessOperation(ILogger logger) : base(logger)
    {
    }

    public event Action<IOperation, Row>? OnRowProcessed
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.OnRowProcessed += value;
            }
        }
        remove
        {
            foreach(var op in this.operations)
            {
                op.OnRowProcessed -= value;
            }
        }
    }
    public event Action<IOperation>? OnFinishedProcessing
    {
        add
        {
            foreach(var op in this.operations)
            {
                op.OnFinishedProcessing += value;
            }
        }
        remove
        {
            foreach (var op in this.operations)
            {
                op.OnFinishedProcessing -= value;           
            }
        }
    }
    
    public void PrepareForExecution()
    {
        
        
        throw new NotImplementedException();
    }

    public IEnumerable<Row> Execute()
    {
        throw new NotImplementedException();
    }

    public void RaiseFinishedProcessing()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);   
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }
        
        foreach(var op in this.operations)
        {
            op.Dispose();
        }
    }

    public IEnumerable<Exception> GetAllErrors()
    {
        return this
            .operations
            .SelectMany(op => op.GetAllErrors());
    }
}