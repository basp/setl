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

// Example5.Run(loggerFactory);

var logger = loggerFactory.CreateLogger<Program>();

var writeOne = new SimpleTransform(
    row =>
    {
        logger.LogInformation("writeOne: {Row}", row);
        return row;
    },
    logger);
    
var writeTwo = new SimpleTransform(
    row =>
    {
        logger.LogInformation("writeTwo: {Row}", row);
        return row;
    },
    logger);
    
var branch = new BranchingOperation(logger);
branch.Add(writeOne);
branch.Add(writeTwo);

var extract = new SimpleExtract(
    () =>
    {
        return Enumerable
            .Range(0, 10)
            .Select(x => new Row() { ["Id"] = x });
    }, 
    logger);

var executor = new SingleThreadedPipelineExecutor(logger);

var process = new SimpleProcess(
    init =>
    {
        init.Register(extract);
        init.Register(branch);
    },
    logger,
    executor);
    
process.Execute();