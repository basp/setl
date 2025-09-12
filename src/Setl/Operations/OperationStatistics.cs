namespace Setl.Operations;

public class OperationStatistics
{
    private DateTime? start;
    private DateTime? end;
    private long outputtedRows;

    public long OutputtedRows => this.outputtedRows;

    public TimeSpan Duration
    {
        get
        {
            if (this.start is null || this.end is null)
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
        Interlocked.Increment(ref this.outputtedRows);
    }

    public void IncrementOutputtedRows()
    {
        Interlocked.Increment(ref this.outputtedRows);
    }

    public override string ToString() =>
        $"{this.outputtedRows} rows, {this.Duration.TotalMilliseconds:0.00} ms";
}