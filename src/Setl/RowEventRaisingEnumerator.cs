using System.Collections;

namespace Setl;

public class RowEventRaisingEnumerator
    : IEnumerable<Row>, IEnumerator<Row>
{
    protected readonly IOperation operation;

    private readonly IEnumerable<Row>? innerEnumerable;
    private IEnumerator<Row>? innerEnumerator;
    private bool disposed;
    
    public RowEventRaisingEnumerator(
        IOperation operation,
        IEnumerable<Row> innerEnumerable)
    {
        this.operation = operation;
        this.innerEnumerable = innerEnumerable;
    }
    
    public Row Current => this.innerEnumerator!.Current;
    
    object IEnumerator.Current => this.innerEnumerator!.Current;

    public virtual bool MoveNext()
    {
        var result = this.innerEnumerator!.MoveNext();
        if (!result)
        {
            return result;
        }
        
        this.operation.RaiseRowProcessed(this.Current);
        return result;
    }

    public void Reset()
    {
        this.innerEnumerator!.Reset();
    }
    
    IEnumerator IEnumerable.GetEnumerator() =>
        ((IEnumerable<Row>)this).GetEnumerator();

    public IEnumerator<Row> GetEnumerator()
    {
        this.ThrowIsMissingInnerEnumerable();
        this.innerEnumerator = this.innerEnumerable!.GetEnumerator();
        return this;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }
        
        if (disposing)
        {
            this.innerEnumerator?.Dispose();
        }
        
        this.disposed = true;
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    private void ThrowIsMissingInnerEnumerable()
    {
        if (this.innerEnumerable != null)
        {
            return;
        }
        
        const string msg = "Null enumerator detected, are you trying to read from the first operation in the process?";
        throw new InvalidOperationException(msg);
    }
}