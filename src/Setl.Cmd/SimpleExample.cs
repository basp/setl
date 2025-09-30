using Setl.Text;

namespace Setl.Cmd;

internal static class SimpleExample
{
    public static void Run()
    {
        var builder = new SequentialTextDeserializerBuilder()
            .Field("foo", 3)
            .Field("bar", 3)
            .Skip(3)
            .Field("quux", 3);

        const string text = @"123XYZ   123";
        var serializer = builder.Build();

        var record1 = serializer.Deserialize<Record>(text);
        Console.WriteLine(
            "[Foo {0}] [Bar {1}] [Quux {2}]",
            record1.foo,
            record1.bar,
            record1.quux);

        var record2 = serializer.Deserialize<MinimalRecord>(text);
        Console.WriteLine("[Foo {0}]", record2.foo);
    }

    private class MinimalRecord
    {
        public string foo { get; set; } = string.Empty;
    }

    private class Record
    {
        public string foo { get; set; } = string.Empty;
        public string bar { get; set; } = string.Empty;
        public string quux { get; set; } = string.Empty;
    }
}