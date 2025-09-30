using System.Text;
using System.Text.RegularExpressions;

namespace Setl.Text.V1;

public class SequentialTextDeserializerBuilder : ITextDeserializerBuilder
{
    private readonly StringBuilder patternBuilder = new();
    private readonly List<string> fields = [];

    public SequentialTextDeserializerBuilder Field(string name, int length)
    {
        this.patternBuilder.Append($"(?<{name}>.{{{length}}})");
        this.fields.Add(name);
        return this;
    }

    public SequentialTextDeserializerBuilder Skip(int length)
    {
        this.patternBuilder.Append($"(.{{{length}}})");
        return this;
    }
    
    public ITextDeserializer Build()
    {
        var pattern = this.patternBuilder.ToString();
        var regex = new Regex(pattern);
        return new TextDeserializer(regex, this.fields.ToArray());
    }
}