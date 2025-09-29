using Microsoft.Extensions.Logging;
using Moq;
using Setl.Operations;

namespace Setl.Tests;

public class EtlProcessTests
{
    [Fact]
    public void TestExecute()
    {
        var logger = new Mock<ILogger>();
        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory
            .Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(logger.Object);
        var executor = new SingleThreadedPipelineExecutor(loggerFactory.Object);
        var process = new TestProcess(executor, loggerFactory.Object);
        process.Execute();
        Assert.Equal(3, process.Count);
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // This needs to be public, otherwise Moq cannot proxy it.
    public class TestProcess : EtlProcess
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly TestLoad load;
        
        public TestProcess(
            IPipelineExecutor pipelineExecutor,
            ILoggerFactory loggerFactory) 
            : base(pipelineExecutor, loggerFactory.CreateLogger<TestProcess>())
        {
            this.loggerFactory = loggerFactory;
            this.load = new TestLoad(loggerFactory);
        }
        
        public int Count => this.load.Count;

        protected override void Initialize()
        {
            var objects = new object[] { 1, 2, 3 };
            this.Register(new TestExtract(this.loggerFactory, objects));
            this.Register(this.load);       
        }
    }

    private class TestLoad : AbstractOperation
    {
        public int Count { get; private set; }

        public TestLoad(ILoggerFactory loggerFactory)
            : base(loggerFactory.CreateLogger<TestLoad>())
        {
        }
        
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
        
        public TestExtract(
            ILoggerFactory loggerFactory,
            params object[] objects)
            : base(loggerFactory.CreateLogger<TestExtract>())
        {
            this.objects = objects;
        }
        
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            return this.objects.Select(Row.FromObject);
        }
    }
}