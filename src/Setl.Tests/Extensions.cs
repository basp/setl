using System.Runtime.CompilerServices;

namespace Setl.Tests;

using Microsoft.Extensions.Logging;
using Moq;

public static class Extensions
{
    public static Mock<ILogger<T>> VerifyDebug<T>(
        this Mock<ILogger<T>> self,
        string message) =>
        VerifyDebug(self, message, Times.Once());

    private static Mock<ILogger<T>> VerifyDebug<T>(
        this Mock<ILogger<T>> self, 
        string message,
        Times times)
    {
        Func<object, Type, bool> state = (v, t) =>
            Equals(v.ToString(), message);
        
        self.Verify(
            x => x.Log(
                It.Is<LogLevel>(level => level == LogLevel.Debug),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>(
                    (v, t) => state(v, t)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>(
                    (v, t) => true)),
            times);

        return self;
    }
}