using System.Collections;

namespace Setl.Enumerables;

public class ThreadSafeEnumerator<T>
    : IEnumerable<T>, IEnumerator<T>
{
    private readonly Queue<T> cached = new Queue<T>();

    private bool active = true;
    private T? current;
    private bool disposed;
    
    public T Current => this.current!;

    public IEnumerator<T> GetEnumerator()
    {
        return this;
    }
      
    object IEnumerator.Current => this.Current!;
  
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        this.Dispose(true);
    }

    public bool MoveNext()
    {
        lock (this.cached)
        {
            while (this.cached.Count == 0 && this.active)
            {
                Monitor.Wait(this.cached);
            }

            if (!this.active && this.cached.Count == 0)
            {
                return false;
            }
            
            this.current = this.cached.Dequeue();
            return true;
        }
    }
    
    public void Reset()
    {
        throw new NotSupportedException();
    }

    public void AddItem(T item)
    {
        lock (this.cached)
        {
            this.cached.Enqueue(item);
            Monitor.Pulse(this.cached);
        }
    }

    public void MarkAsFinished()
    {
        lock (this.cached)
        {
            this.active = false;
            Monitor.Pulse(this.cached);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (this.disposed)
        {
            return;
        }

        if (disposing)
        {
            this.cached.Clear();
        }
        
        this.disposed = true;
    }
}