namespace Setl.Operations;

public class OperationStatistics
{
    private DateTime? start;
    private DateTime? end;
    private long processed;

    public long Processed => this.processed;
    
    public TimeSpan Duration
    {
        get
        {
            if (this.start == null || this.end == null)
            {
                return TimeSpan.Zero;
            }
            
            return this.end.Value - this.start.Value;
        }
    }

    public void MarkStarted()
    {
        this.start = DateTime.Now;
    }
    
    public void MarkFinished()
    {
        this.end = DateTime.Now;
    }

    public void MarkRowProcessed()
    {
        Interlocked.Increment(ref this.processed);
    }

    public override string ToString() =>
        $"Processed {this.processed} rows in {this.Duration}";
}