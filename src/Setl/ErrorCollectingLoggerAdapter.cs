using Microsoft.Extensions.Logging;

namespace Setl;

/// <summary>
/// Provides a thin wrapper around a <see cref="ILogger"/> to collect
/// any <see cref="Exception"/> instances that are passed to the <c>Log</c>
/// method. This is useful if there's any post-processing work that clients
/// want to do with the errors.
/// </summary>
/// <remarks>
/// This class is used internally by the ETL framework. The exceptions are
/// exposed by the <c>GetErrors</c> methods provided by various components of
/// the framework.
/// </remarks>
internal class ErrorCollectingLoggerAdapter : ILogger
{
    private readonly ILogger logger;
    private readonly List<Exception> errors = [];
    
    public ErrorCollectingLoggerAdapter(ILogger logger)
    {
        this.logger = logger;
    }

    public IEnumerable<Exception> GetErrors() => this.errors;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?,
            string> formatter)
    {
        if (exception is not null)
        {
            var error = new EtlException(
                "Error during ETL process.",
                exception);
            this.errors.Add(error);
        }
    }

    public bool IsEnabled(LogLevel logLevel) => 
        this.logger.IsEnabled(logLevel);

    public IDisposable? BeginScope<TState>(TState state) 
        where TState : notnull => this.logger.BeginScope(state);
}