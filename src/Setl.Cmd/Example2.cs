using Microsoft.Extensions.Logging;
using Setl;

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
            foreach (var foo in this.sourceFoo)
            {
                yield return Row.FromObject(foo);
            }
        }
    }
    
    internal class Foo
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    
    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<ProcessNumbersExample>(); 
        // var executor = new SingleThreadedNonCachedPipelineExecutor(logger);
        var executor = new SingleThreadedPipelineExecutor(logger);
        var process = new ProcessNumbersExample(logger, executor);
        process.Execute();
    }
}