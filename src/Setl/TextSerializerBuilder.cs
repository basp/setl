using System.Text;
using System.Text.RegularExpressions;

namespace Setl;

public class TextSerializerBuilder
{
    public bool IsZeroIndexed { get; set; } = false;

    // private int currentOffset = 0;

    private readonly StringBuilder patternBuilder = new();
    private readonly List<string> fields = [];

    public TextSerializerBuilder TextField(string name, int length)
    {
        this.patternBuilder.Append($"(?<{name}>.{{{length}}})");
        this.fields.Add(name);
        return this;
    }

    public TextSerializerBuilder Skip(int length)
    {
        this.patternBuilder.Append($"(.{{{length}}})");
        return this;
    }
    
    public TextSerializer Build()
    {
        var pattern = this.patternBuilder.ToString();
        var regex = new Regex(pattern);
        return new TextSerializer(regex, this.fields.ToArray());
    }
}