using Moq;

namespace Setl.Tests;

using Microsoft.Extensions.Logging;

public class EtlProcessTests
{
    public class TestEtlProcess : EtlProcess
    {
        public TestEtlProcess(ILogger<TestEtlProcess> logger) : base(logger)
        {
        }
    }

    [Fact]
    public void RegistrationLogsDebugMessage()
    {
        var logger = new Mock<ILogger<TestEtlProcess>>();
        var process = new TestEtlProcess(logger.Object);
        
        var operation = new Mock<IOperation>();
        operation
            .Setup(o => o.Name)
            .Returns("EtlMockOperation");
        process.Register(operation.Object);
        
        // https://adamstorr.co.uk/blog/mocking-ilogger-with-moq/
        logger.Verify(
            x => x.Log(
                It.Is<LogLevel>(level => level == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), 
            Times.Once());
    }
}