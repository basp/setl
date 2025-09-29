namespace Setl.Enumerables;

public class EventRaisingEnumerator : RowEventRaisingEnumerator
{
    public EventRaisingEnumerator(
        IOperation operation, 
        IEnumerable<Row> innerEnumerable) 
        : base(operation, innerEnumerable)
    {
    }

    public override bool MoveNext()
    {
        var result = base.MoveNext();
        if (!result)
        {
            this.operation.RaiseFinishedProcessing();
        }

        return result;
    }
}