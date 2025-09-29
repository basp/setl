using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class EtlProcessBase<T>
    where T : EtlProcessBase<T>
{
    private readonly List<IOperation> lastOperations = [];
    protected readonly List<IOperation> operations = [];

    private readonly ILogger logger;
    
    protected EtlProcessBase(ILogger logger)
    {
        this.logger = logger;
    }
    
    public string Name => this.GetType().Name;
    
    public T Register(IOperation operation)
    {
        this.operations.Add(operation);
        return (T)this;
    }
    
    public T RegisterLast(IOperation operation)
    {
        this.lastOperations.Add(operation);
        return (T)this;
    }

    protected void MergeLastOperations()
    {
        this.operations.AddRange(this.lastOperations);
    }
}