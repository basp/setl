namespace Setl;

public class EtlProcessBase<T>
    where T : EtlProcessBase<T>
{
    private readonly List<IOperation> lastOperations = [];
    private readonly List<IOperation> operations = [];

    public string Name => this.GetType().Name;
}