using Microsoft.Extensions.Logging;
using Setl;

using var loggerFactory = 
    LoggerFactory.Create(builder => 
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
                cfg.SingleLine = false;
            })
            .SetMinimumLevel(LogLevel.Trace));

// Example1.Run(loggerFactory);
Example2.Run(loggerFactory);

internal static class Example2
{
    private class ProcessNumbersExample : EtlProcess
    {
        private readonly ILogger logger;
        
        public ProcessNumbersExample(
            ILogger logger, 
            IPipelineExecutor executor)
        : base(logger, executor)
        {        
            this.logger = logger;
        }

        protected override void Initialize()
        {
            this.Register(new ExtractFakeData(this.logger));
            this.Register(new WriteFakeData(this.logger));
        }
    }

    private class WriteFakeData : AbstractOperation
    {
        private readonly ILogger logger;
    
        // Mandatory `ILogger` ctor
        public WriteFakeData(ILogger logger) : base(logger)
        {
            this.logger = logger;
        }

        // Custom name (optional)
        public override string Name => "write-fake-data";


        // Yield our fake object data source as `Row` instances
        public override IEnumerable<Row> Execute(
            IEnumerable<Row> rows)
        {
            foreach(var row in rows)
            {
                this.logger.LogInformation( "Write: {row}", row);
                yield return row;
            }
        }
    }
    
    private class ExtractFakeData : AbstractOperation
    {
        // A hard coded *fake-data* source
        private readonly List<Foo> sourceFoo =
        [
            new() { Id = 1, Name = "Foo_One" },
            new() { Id = 2, Name = "Foo_Two" },
            new() { Id = 3, Name = "Foo_Three" },
        ];

        // Mandatory `ILogger` ctor
        public ExtractFakeData(ILogger logger) : base(logger)
        {
        }
    
        // Custom name (optional)
        public override string Name => "extract-fake-data";


        // Yield our fake object data source as `Row` instances
        public override IEnumerable<Row> Execute(
            IEnumerable<Row> rows)
        {
            foreach (var foo in sourceFoo)
            {
                yield return Row.FromObject(foo);
            }
        }
    }
    
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<ProcessNumbersExample>(); 
        var executor = new SingleThreadedNonCachedPipelineExecutor(logger);
        var process = new ProcessNumbersExample(logger, executor);
        process.Execute();
    }
}

internal static class Example1
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<ExampleEtlProcess>(); 
        var executor = new SingleThreadedNonCachedPipelineExecutor(logger);
        var process = new ExampleEtlProcess(logger, executor);
        process.Execute();
    }
}

internal class Foo
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

internal class Bar
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int FooId { get; set; }
}

internal class ExampleEtlProcess : EtlProcess
{
    private readonly ILogger logger;
    
    public ExampleEtlProcess(
        ILogger logger, 
        IPipelineExecutor pipelineExecutor) 
        : base(logger, pipelineExecutor)
    {
        this.logger = logger;
    }

    public override string Name => "example-etl-process";

    protected override void Initialize()
    {
        this.Register(new ExtractFooRecords(this.logger));
        this.Register(new ConvertNameToUpperCase(this.logger));
        this.Register(new WriteFooRecords("write-before-validate", this.logger));
        this.Register(new ValidateFooRecords(this.logger));
        this.Register(new HashFooRecords(this.logger));
        this.Register(new SplitFooNames(this.logger));
        this.Register(new WriteFooRecords("final-write", this.logger)
        {
            IsFinal = true,
        });
    }

    protected override void PostProcessing()
    {
        this.logger.LogInformation("No post processing required");
    }

    protected override void OnRowProcessed(IOperation op, Row row)
    {
        this.logger.LogTrace(
            "Row processed [{Operation}]: {Row}", 
            op.Name, 
            row);
    }

    protected override void OnFinishedProcessing(IOperation op)
    {
        this.logger.LogTrace(
            "Finished processing operation {Operation}", 
            op.Name);
    }
}

internal class ValidateFooRecords : AbstractOperation
{
    public ValidateFooRecords(ILogger logger) : base(logger)
    {
    }

    public override string Name => "validate-foo-records";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        const string pattern = "TWO";
        foreach (dynamic row in rows)
        {
            if (row.Name.Contains(pattern))
            {
                this.LogInformation(
                    "Name contains {Pattern}; skipping", 
                    pattern);
                continue;
            }

            yield return row;
        }
    }
}

internal class WriteFooRecords : AbstractOperation
{
    public WriteFooRecords(string name, ILogger logger) : base(logger)
    {
        this.Name = name;
    }

    public override string Name { get; }
    
    public bool IsFinal { get; set; }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        var rowCount = 0;
        foreach (var row in rows)
        {
            rowCount += 1;
            this.LogInformation(
                "#{RowCount} {Operation}: {Row}",
                rowCount,
                this.GetType().Name, 
                row);
            row["IsFinal"] = this.IsFinal;
            yield return row;
        }
    }
}

internal class HashFooRecords : AbstractOperation
{
    public HashFooRecords(ILogger logger) : base(logger)
    {
    }
    
    public override string Name => "hash-records";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (dynamic row in rows)
        {
            var updated = row.Clone();
            updated.Hash = updated.Id.GetHashCode();
            yield return updated;
        }
    }
}

internal class SplitFooNames : AbstractOperation
{
    public SplitFooNames(ILogger logger) : base(logger)
    {
    }
    
    public override string Name => "split-names";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (dynamic row in rows)
        {
            var updated = row.Clone();
            var parts = row.Name.Split('_');
            updated.FirstName = parts[0];
            updated.LastName = parts[1];
            yield return updated;
        }
    }
}

internal class ExtractBarRecords : AbstractOperation
{
    private readonly List<Bar> sourceBar =
    [
        new()
        {
            Id = 1,
            Name = "Bar_One",
            FooId = 1,
        },
    ];

    public ExtractBarRecords(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        throw new NotImplementedException();
    }
}

internal class ConvertNameToUpperCase : AbstractOperation
{
    public ConvertNameToUpperCase(ILogger logger) : base(logger)
    {
    }
    
    public override string Name => "name-to-upper-case";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (dynamic row in rows)
        {
            var updated = row.Clone();
            updated.Name = row["Name"].ToUpper();
            yield return updated;
        }
    }
}

internal class ExtractFooRecords : AbstractOperation
{
    private readonly List<Foo> sourceFoo =
    [
        new() { Id = 1, Name = "Foo_One" },
        new() { Id = 2, Name = "Foo_Two" },
        new() { Id = 3, Name = "Foo_Three" },
    ];

    public ExtractFooRecords(ILogger logger) : base(logger)
    {
    }
    
    public override string Name => "extract-foo-records";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var foo in sourceFoo)
        {
            yield return Row.FromObject(foo);
        }
    }
}