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

// Example1.Run(loggerFactory);
// Example2.Run(loggerFactory);
// Example3.Run(loggerFactory);
Example4.Run(loggerFactory);

internal class FakeExtract : AbstractOperation
{
    private readonly IEnumerable<object> subjects;
    
    public FakeExtract(IEnumerable<object> subjects, ILogger logger) 
        : base(logger)
    {
        this.subjects = subjects;
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> _) =>
        this.subjects.Select(Row.FromObject);
}

internal class WriteRow : AbstractOperation
{
    public WriteRow(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach(var row in rows)
        {
            this.LogDebug("Row: {Row}", row);
            yield return row;
        }
    }
}

internal class TestAggregate : AbstractAggregationOperation
{
    public TestAggregate(ILogger logger) : base(logger)
    {
    }

    protected override void Accumulate(Row row, Row aggregate)
    {
        if (!row.TryGetString("Category", out var category))
        {
            this.LogWarning("Row does not contain Category column.");
            return;
        }
        
        const string categoryKey = "category";
        const string groupKey = "group";
        
        if (!aggregate.ContainsKey(groupKey))
        {
            aggregate[categoryKey] = category;
            aggregate[groupKey] = new List<Row>();
        }

        
        var group = aggregate[groupKey] as List<Row>;
        group?.Add(row);
    }
    
    protected override string[] GetColumnsToGroupBy() => ["Category"];
}

internal class CountSubjects : AbstractOperation
{
    public CountSubjects(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (dynamic row in rows)
        {
            row.Count = row.Group.Count;
            yield return row;
        }
    }
}

internal class Example4
{
    private class TestAggregationProcess : EtlProcess
    {
        private readonly ILogger logger;
        private readonly Action<EtlProcess> register;
        
        public TestAggregationProcess(
            ILogger logger, 
            IPipelineExecutor pipelineExecutor,
            Action<EtlProcess> register) 
            : base(logger, pipelineExecutor)
        {
            this.logger = logger;
            this.register = register;
        }

        protected override void Initialize()
        {
            this.register(this);
        }
    }
    
    private class Subject
    {
        public int Id { get; set; }
        
        public string? Name { get; set; }

        public string? Category { get; set; }
    }

    private static readonly List<Subject> subjects =
    [
        new()
        {
            Id = 1,
            Name = "Subject_One",
            Category = "Category_One",
        },
        new()
        {
            Id = 2,
            Name = "Subject_Two",
            Category = "Category_One",
        },
        new()
        {
            Id = 3,
            Name = "Subject_Three",
            Category = "Category_Two",
        },
        new()
        {
            Id = 4,
            Name = "Subject_Four",
            Category = "Category_Two",
        },
        new()
        {
            Id = 5,
            Name = "Subject_Five",
            Category = "Category_Three",
        },
        new()
        {
            Id = 6,
            Name = "Subject_Six",
            Category = "Category_One",
        }
    ];

    public static void Run(ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<Example4>();
        var extract = new FakeExtract(subjects, logger);
        var aggregate = new TestAggregate(logger);
        var count = new CountSubjects(logger);
        var write = new WriteRow(logger);
        var process = new TestAggregationProcess(
            logger, 
            new SingleThreadedPipelineExecutor(logger), 
            p =>
            {
                p.Register(extract);
                p.Register(aggregate);
                p.Register(count);
                p.Register(write);
            });
        process.Execute();
    }
}