using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Setl;

[SuppressMessage(
    "Usage", 
    "CA2254:Template should be a static expression",
    Justification = "Code using this class should be extra careful.")]
public class LoggerAdapter
{
    private readonly ILogger logger;
    private readonly List<Exception> errors = [];
    
    protected LoggerAdapter(ILogger logger)
    {
        this.logger = logger;
    }
    
    protected Exception[] Errors => this.errors.ToArray();

    protected void LogTrace(string message, params object[] args)
    {
        if (this.logger.IsEnabled(LogLevel.Trace))
        {
            this.logger.LogTrace(message, args);
        }
    }
    
    protected void LogDebug(string message, params object[] args)
    {
        if (this.logger.IsEnabled(LogLevel.Debug))
        {
            this.logger.LogDebug(message, args);
        }
    }

    protected void LogInformation(string message, params object[] args)
    {
        if (this.logger.IsEnabled(LogLevel.Information))
        {
            this.logger.LogInformation(message, args);
        }
    }

    protected void LogWarning(string message, params object[] args)
    {
        if (this.logger.IsEnabled(LogLevel.Warning))
        {
            this.logger.LogWarning(message, args);
        }
    }
    
    protected void LogError(
        Exception ex, 
        string message, 
        params object[] args)
    {
        this.errors.Add(ex);
        if (this.logger.IsEnabled(LogLevel.Error))
        {
            this.logger.LogError(ex, message, args);
        }
    }
}