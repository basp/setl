using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class EtlProcessBase<T>
    where T : EtlProcessBase<T>
{
    private readonly List<IOperation> lastOperations = [];
    protected readonly List<IOperation> operations = [];

    internal readonly ErrorCollectingLoggerAdapter logger;
    
    protected EtlProcessBase(ILogger logger)
    {
        this.logger = new ErrorCollectingLoggerAdapter(logger);
    }
    
    public string Name => this.GetType().Name;
    
    public T Register(IOperation operation)
    {
        this.operations.Add(operation);
        this.logger.LogDebug(
            "Register {Operation} in {Process}", 
            operation.Name, 
            this.Name);
        return (T)this;
    }
    
    public T RegisterLast(IOperation operation)
    {
        this.lastOperations.Add(operation);
        this.logger.LogDebug(
            "Register {Operation} (last) in {Process}",
            operation.Name,
            this.Name);
        return (T)this;
    }

    protected void MergeLastOperations()
    {
        this.operations.AddRange(this.lastOperations);
    }
}