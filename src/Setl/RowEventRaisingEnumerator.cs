using System.Collections;

namespace Setl;

public class RowEventRaisingEnumerator
    : IEnumerable<Row>, IEnumerator<Row>
{
    protected readonly IOperation operation;

    private readonly IEnumerable<Row>? inner;
    
    private IEnumerator<Row>? innerEnumerator;
    private bool disposed;

    public RowEventRaisingEnumerator(
        IOperation operation,
        IEnumerable<Row> inner)
    {
        this.operation = operation;
        this.inner = inner;
    }

    public Row Current => this.innerEnumerator!.Current;

    object? IEnumerator.Current => this.innerEnumerator!.Current;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    public virtual bool MoveNext()
    {
        var result = this.innerEnumerator!.MoveNext();
        if (!result)
        {
            return result;
        }
        
        // Apparently we're not doing anything with the previous row?
        // TODO:
        // Make sure this works as expected and that we *really* don't need
        // the previous row.
        _ = this.innerEnumerator!.Current;
        this.operation.RaiseRowProcessed(this.Current);
        return result;
    }

    public void Reset()
    {
        this.innerEnumerator!.Reset();
    }


    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<Row>)this).GetEnumerator();
    }

    IEnumerator<Row> IEnumerable<Row>.GetEnumerator()
    {
        this.ThrowIfMissingInner();
        this.innerEnumerator = this.inner!.GetEnumerator();
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

    private void ThrowIfMissingInner()
    {
        if (this.inner != null)
        {
            return;
        }
        
        const string msg = "Null enumerator detected, are you trying to read from the first operation in the process?";
        throw new InvalidOperationException(msg);
    }
}