using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Setl.Cmd.Examples;

var loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
                cfg.SingleLine = true;
                cfg.ColorBehavior = LoggerColorBehavior.Enabled;
            })
            .SetMinimumLevel(LogLevel.Information));

using (loggerFactory)
{
    AvdExample.Run(loggerFactory);
    // Setl.Cmd.SVBWWB65PlusProcessExample.Run(loggerFactory);
    // Example3.Run(loggerFactory);
    // Setl.Cmd.Examples.V2.TextDeserializerExample.Run();
    // AowAioExample.Run();
}
