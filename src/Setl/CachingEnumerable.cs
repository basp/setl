using System.Collections;

namespace Setl;

public class CachingEnumerable<T> 
    : IEnumerable<T>, IEnumerator<T>
{
    private bool? isFirstTime = null;
    private IEnumerator<T> internalEnumerator;
    private readonly LinkedList<T> cache = [];
    
    public CachingEnumerable(IEnumerable<T> inner)
    {
        this.internalEnumerator = inner.GetEnumerator();
    }
    
    T IEnumerator<T>.Current => this.internalEnumerator.Current;

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        if (this.isFirstTime == null)
        {
            this.isFirstTime = true;
        }
        else if (this.isFirstTime.Value)
        {
            this.isFirstTime = false;
            this.internalEnumerator.Dispose();
            this.internalEnumerator = this.cache.GetEnumerator();
        }
        else
        {
            this.internalEnumerator = this.cache.GetEnumerator();
        }

        return this;
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    public bool MoveNext()
    {
        var result = this.internalEnumerator.MoveNext();
        if (result && this.isFirstTime.HasValue && this.isFirstTime.Value)
        {
            this.cache.AddLast(this.internalEnumerator.Current);
        }

        return result;
    }
    
    public object? Current => this.internalEnumerator.Current;
    
    public void Reset()
    {
        this.internalEnumerator.Reset();
    }

    public void Dispose()
    {
        this.internalEnumerator.Dispose();
    }
}