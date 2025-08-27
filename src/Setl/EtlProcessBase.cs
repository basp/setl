namespace Setl;

using Microsoft.Extensions.Logging;

public class EtlProcessBase<T>
    where T : EtlProcessBase<T>
{
    private readonly ILogger logger;
    
    private readonly List<IOperation> lastOperations = [];
    
    protected readonly List<IOperation> operations = [];

    protected EtlProcessBase(ILogger logger)
    {
        this.logger = logger;
    }

    public bool UseTransaction { get; set; } = true;
    
    public virtual string Name => this.GetType().Name;
    
    public T Register(IOperation operation)
    {
        operation.UseTransaction = this.UseTransaction;
        this.operations.Add(operation);
        this.logger.LogDebug(
            "Register {Operation} in {Process}",
            operation.Name,
            this.Name);;
        return (T)this;
    }

    public T RegisterLast(IOperation operation)
    {
        this.lastOperations.Add(operation);
        this.logger.LogDebug(
            "RegisterLast {Operation} in {Process}",
            operation.Name,
            this.Name);
        return (T)this;
    }

    protected void MergeLastOperations()
    {
        this.operations.AddRange(this.lastOperations);
    }
}