using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Examples;

internal static class ETlProcessExample
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var executor = new SingleThreadedPipelineExecutor(loggerFactory);
        var process = new TestProcess(executor, loggerFactory);
        process.Execute();
    }
    
    private class TestProcess : EtlProcess
    {
        private ILoggerFactory loggerFactory;
        
        public TestProcess(
            IPipelineExecutor pipelineExecutor,
            ILoggerFactory loggerFactory) 
            : base(pipelineExecutor, loggerFactory.CreateLogger<TestProcess>())
        {
            this.loggerFactory = loggerFactory;
        }

        protected override void Initialize()
        {
            this.Register(new Extract(this.loggerFactory));
            this.Register(new Write(this.loggerFactory));
        }
    }
    
    private class Extract : AbstractOperation
    {
        public Extract(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<Extract>())
        {
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            var objects = new[]
            {
                new
                {
                    Id = 1,
                    Name = "John",
                },
                new
                {
                    Id = 2,
                    Name = "Jane",
                },
            };

            return objects.Select(Row.FromObject);
        }
    }

    private class Write : AbstractOperation
    {
        public Write(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<Write>())
        {
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                Console.WriteLine(row.ToJson());
            }

            yield break;
        }
    }
}