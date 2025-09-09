using Microsoft.Extensions.Logging;
using Setl;

using var loggerFactory = 
    LoggerFactory.Create(builder => 
        builder.AddConsole());

var executor = new SingleThreadedNonCachedPipelineExecutor(loggerFactory);
var process = new ExampleEtlProcess(
    loggerFactory.CreateLogger<ExampleEtlProcess>(), 
    executor);

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

    protected override void Initialize()
    {
        this.Register(new ExtractFooRecords(this.logger));
        this.Register(new WriteFooRecords(this.logger));
    }
}

internal class WriteFooRecords : AbstractOperation
{
    public WriteFooRecords(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            Console.WriteLine(row);
        }

        return [];
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

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var foo in sourceFoo)
        {
            yield return Row.FromObject(foo);
        }
    }
}