using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Setl;

using var loggerFactory = 
    LoggerFactory.Create(builder => 
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
            })
            .SetMinimumLevel(LogLevel.Trace));

var logger = loggerFactory.CreateLogger<ExampleEtlProcess>(); 
var executor = new SingleThreadedNonCachedPipelineExecutor(logger);
var process = new ExampleEtlProcess(logger, executor);

process.Execute();

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

    public override string Name => "Example ETL Process";

    protected override void Initialize()
    {
        this.Register(new ExtractFooRecords(this.logger));
        this.Register(new ConvertNameToUpperCase(this.logger));
        this.Register(new ValidateFooRecords(this.logger));
        this.Register(new HashFooRecords(this.logger));
        this.Register(new SplitFooNames(this.logger));
        this.Register(new WriteFooRecords(this.logger));
    }
}

internal class ValidateFooRecords : AbstractOperation
{
    public ValidateFooRecords(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        const string pattern = "TWO";
        foreach (dynamic row in rows)
        {
            if (row.Name.Contains(pattern))
            {
                this.LogInformation("Name contains {Pattern}", pattern);
            }

            yield return row;
        }
    }
}

internal class WriteFooRecords : AbstractOperation
{
    public WriteFooRecords(ILogger logger) : base(logger)
    {
    }

    public override string Name => "Write Foo Records";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            Console.WriteLine(row);
        }

        return [];
    }
}

internal class HashFooRecords : AbstractOperation
{
    public HashFooRecords(ILogger logger) : base(logger)
    {
    }
    
    public override string Name => "Hash Foo Records";

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
    
    public override string Name => "Split Foo Names";

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
    
    public override string Name => "Convert Name To Upper Case";

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
    
    public override string Name => "Extract Foo Records";

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var foo in sourceFoo)
        {
            yield return Row.FromObject(foo);
        }
    }
}