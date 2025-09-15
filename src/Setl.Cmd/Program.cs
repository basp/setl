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

var builder = new TextSerializerBuilder()
    .TextField("foo", 3)
    .TextField("bar", 3)
    .Skip(3)
    .TextField("quux", 3);

var serializer = builder.Build();
var text = @"123XYZ   123";

var record1 = serializer.Deserialize<Record>(text);
Console.WriteLine(
    "[Foo {0}] [Bar {1}] [Quux {2}]", 
    record1.foo, 
    record1.bar,
    record1.quux);

var record2 = serializer.Deserialize<MinimalRecord>(text);
Console.WriteLine("[Foo {0}]", record2.foo);

public class MinimalRecord
{
    public string foo { get; set; } = string.Empty;
}

public class Record
{
    public string foo { get; set; } = string.Empty;
    public string bar { get; set; } = string.Empty;
    public string quux { get; set; } = string.Empty;
}