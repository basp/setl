using Microsoft.Extensions.Logging;
using Setl;

internal static class ETlProcessExample
{
    public static void Run(ILoggerFactory loggerFactory)
    {
        var executor = new SingleThreadedPipelineExecutor(loggerFactory);
        var process = new TestProcess(executor);
        process.Execute();
    }
    
    private class TestProcess : EtlProcess
    {
        public TestProcess(IPipelineExecutor pipelineExecutor) 
            : base(pipelineExecutor)
        {
        }

        protected override void Initialize()
        {
            this.Register(new Extract());
            this.Register(new Write());
        }
    }
    
    private class Extract : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            yield return new Row();
            yield return new Row();
        }
    }

    private class Write : AbstractOperation
    {
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