namespace Setl;

using Microsoft.Extensions.Logging;

public class OperationStats
{
    // private DateTime? start;
    // private DateTime? end;
    // private long outputtedRows = 0;
    private readonly ILogger<OperationStats> logger;

    public OperationStats(ILogger<OperationStats> logger)
    {
        this.logger = logger;
    }

    public void MarkStarted()
    {
        this.logger.LogTrace("MarkStarted");
    }

    public void MarkFinished()
    {
        this.logger.LogTrace("MarkFinished");
    }
    
    public void MarkRowProcessed()
    {
        this.logger.LogTrace("MarkRowProcessed");
    }
}