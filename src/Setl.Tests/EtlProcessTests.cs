using Moq;
using Setl.Operations;

namespace Setl.Tests;

using Microsoft.Extensions.Logging;

public class EtlProcessTests
{
    public class TestEtlProcess : EtlProcess
    {
        public TestEtlProcess(
            ILogger<TestEtlProcess> logger,
            IPipelineExecutor pipelineExecutor) 
            : base(logger, pipelineExecutor)
        {
        }

        protected override void Initialize()
        {
        }
    }

    [Fact]
    public void RegistrationLogsDebugMessage()
    {
        var logger = new Mock<ILogger<TestEtlProcess>>();
        var executor = new Mock<IPipelineExecutor>();
        var process = new TestEtlProcess(logger.Object, executor.Object);
        var operation = new Mock<IOperation>();
        operation
            .Setup(o => o.Name)
            .Returns(nameof(EtlProcessTests));
        
        process.Register(operation.Object);
        
        // We cannot verify on extension methods, so verify the low-level `Log`
        // method instead.
        // https://adamstorr.co.uk/blog/mocking-ilogger-with-moq/
        //
        // logger.Verify(
        //     x => x.Log(
        //         It.Is<LogLevel>(level => level == LogLevel.Debug),
        //         It.IsAny<EventId>(),
        //         It.Is<It.IsAnyType>((v, t) => true),
        //         It.IsAny<Exception>(),
        //         It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), 
        //     Times.Once());
        //
        logger.VerifyDebug(
            $"Register {nameof(EtlProcessTests)} in {nameof(TestEtlProcess)}");
    }
    
    [Fact]
    public void RegisterLastLogsDebugMessage()
    {
        var logger = new Mock<ILogger<TestEtlProcess>>();
        var pipelineExecutor = new Mock<IPipelineExecutor>();
        var process = new TestEtlProcess(logger.Object,  pipelineExecutor.Object);
        var operation = new Mock<IOperation>();
        operation
            .Setup(o => o.Name)
            .Returns(nameof(EtlProcessTests));
        
        process.RegisterLast(operation.Object);
        
        logger.VerifyDebug(
            $"RegisterLast {nameof(EtlProcessTests)} in {nameof(TestEtlProcess)}");       
    }
}