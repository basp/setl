using Setl.Operations;

namespace Setl.Enumerables;

public class EventRaisingEnumerator : SingleRowEventRaisingEnumerator
{
    public EventRaisingEnumerator(
        IOperation operation, 
        IEnumerable<Row> inner) 
        : base(operation, inner)
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