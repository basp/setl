using System.Text;
using System.Text.RegularExpressions;

namespace Setl;

public class SequentialTextSerializerBuilder : ITextSerializerBuilder
{
    private readonly StringBuilder patternBuilder = new();
    private readonly List<string> fields = [];

    public SequentialTextSerializerBuilder TextField(string name, int length)
    {
        this.patternBuilder.Append($"(?<{name}>.{{{length}}})");
        this.fields.Add(name);
        return this;
    }

    public SequentialTextSerializerBuilder Skip(int length)
    {
        this.patternBuilder.Append($"(.{{{length}}})");
        return this;
    }
    
    public ITextSerializer Build()
    {
        var pattern = this.patternBuilder.ToString();
        var regex = new Regex(pattern);
        return new TextSerializer(regex, this.fields.ToArray());
    }
}