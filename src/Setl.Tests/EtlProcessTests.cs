using Microsoft.Extensions.Logging;
using Moq;

namespace Setl.Tests;

public class EtlProcessTests
{
    [Fact]
    public void TestExecute()
    {
        var loggerFactory = new Mock<ILoggerFactory>();
        var executor = new SingleThreadedPipelineExecutor(loggerFactory.Object);
        var process = new TestProcess(executor);
        process.Execute();
        Assert.Equal(3, process.Count);
    }

    private class TestProcess : EtlProcess
    {
        private static readonly TestLoad load = new();
        
        public TestProcess(IPipelineExecutor pipelineExecutor) 
            : base(pipelineExecutor)
        {
        }
        
        public int Count => TestProcess.load.Count;

        protected override void Initialize()
        {
            var objects = new object[] { 1, 2, 3 };
            this.Register(new TestExtract(objects));
            this.Register(TestProcess.load);       
        }
    }

    private class TestLoad : AbstractOperation
    {
        public int Count { get; private set; }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (var row in rows)
            {
                this.Count += 1;
                yield return row;
            }
        }
    }
    
    private class TestExtract : AbstractOperation
    {
        private readonly object[] objects;
        
        public TestExtract(params object[] objects)
        {
            this.objects = objects;
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            return this.objects.Select(Row.FromObject);
        }
    }
}