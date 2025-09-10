using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Setl;

using var loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
                cfg.SingleLine = true;
            })
            .SetMinimumLevel(LogLevel.Trace));

Example5.Run(loggerFactory);

internal static class Example5
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Simple ETL");

        var executor = new SingleThreadedPipelineExecutor(logger);
        
        var extract = new NamedOperation(
            "setl-extract", 
            new SimpleExtract(
                () =>
                {
                    return Enumerable
                        .Range(0, 1)
                        .Select(x => new Row() { ["Id"] = x });
                }, 
                logger), 
            logger);

        var transform = new NamedOperation(
            "setl-transform-1",
            new SimpleTransform(
                row =>
                {
                    logger.LogInformation("Transform: {Row}", row);
                    row["Transformed"] = DateTime.UtcNow;
                    return row;
                },
                logger),
            logger);

        var delay = new Func<int, SimpleTransform>(ms => 
            new SimpleTransform(
                row =>
                {
                    Thread.Sleep(ms);
                    return row;
                }, 
                logger));
        
        var dynTransform = new SimpleDynamicTransform(
            row =>
            {
                logger.LogInformation("Processed: {Row}", (Row)row);
                row.Processed = DateTime.UtcNow;
                return row;
            },
            logger);

        var load = new SimpleTransform(
            row =>
            {
                logger.LogInformation("Load: {Row}", row);
                return row;
            },
            logger);

        var process = new SimpleProcess(ops =>
            {
                ops.Register(extract);
                ops.Register(transform);
                ops.Register(
                    new NamedOperation(
                        "delay-1",
                        delay(5000),
                        logger));
                ops.Register(
                    new NamedOperation(
                        "setl-dyn-transform",
                        dynTransform,
                        logger));
                ops.Register(
                    new NamedOperation(
                        "delay-2",
                        delay(5000),
                        logger));
                ops.Register(
                    new NamedOperation(
                        "setl-load",
                        load,
                        logger));
                ops.Register(
                    new NamedOperation(
                        "delay-3",
                        delay(5000),
                        logger));
            },
            logger,
            executor);
    
        process.Execute();
    }
}
