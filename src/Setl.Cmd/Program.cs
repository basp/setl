using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;
using Setl.Pipelines;

using var loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
                cfg.SingleLine = true;
            })
            .SetMinimumLevel(LogLevel.Trace));

Example5.Run(loggerFactory);
