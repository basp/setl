using Microsoft.Extensions.Logging;
using Setl;
using Setl.Operations;
using Setl.Pipelines;

internal static class Example5
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("Simple ETL");

        var executor = new SingleThreadedPipelineExecutor(logger);
        
        // We can name operations explicitly by wrapping them in a
        // `NamedOperation` at definition time. I don't really like this
        // style since it's a bit overly verbose. It's a bit cleaner to
        // apply the wrappers at registration time (this also prevents the
        // double `logger' injection).
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

        // However, we can also just define our *base* (unnamed)
        // transformations.
        var transform = new SimpleTransform(
            row =>
            {
                logger.LogInformation("Transform: {Row}", row);
                row["Transformed"] = DateTime.UtcNow;
                return row;
            },
            logger);
        
        var dynTransform = new SimpleDynamicTransform(
            row =>
            {
                logger.LogInformation("Process: {Row}", (Row)row);
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


        var delay = new Func<int, SimpleTransform>(ms => 
            new SimpleTransform(
                row =>
                {
                    Thread.Sleep(ms);
                    return row;
                }, 
                logger));

        var process = new SimpleProcess(ops =>
            {
                ops.Register(extract);
                
                // And we can also just wait to wrap our operations when
                // we are registering them. Personally, I prefer applying
                // wrappers at this point. It's a bit more declarative, and
                // your core operations are more cleanly defined.
                ops.Register(
                    new NamedOperation(
                        "setl-transform",
                        transform,
                        logger));
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
