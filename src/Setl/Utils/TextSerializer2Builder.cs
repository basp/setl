using System.Text;
using System.Text.RegularExpressions;

namespace Setl.Utils;

public class TextSerializer2Builder
{
    private readonly List<TextField> fields = [];

    public TextSerializer2Builder Field(string name, int length) =>
        this.Field(field =>
        {
            field.Name = name;
            field.Length = length;
        });
    
    public TextSerializer2Builder Field(Action<TextField> configure)
    {
        var field = new TextField();
        configure(field);
        this.fields.Add(field);
        return this;
    }

    public TextSerializer2 Build()
    {
        var patternBuilder = new StringBuilder();
        foreach (var field in this.fields)
        {
            var pattern = field.Skip
                ? $"(.{{{field.Length}}})"
                : $"(?<{field.Name}>.{{{field.Length}}})";
            
            patternBuilder.Append(pattern);
        }
        
        var regex = new Regex(patternBuilder.ToString());
        return new TextSerializer2(regex, this.fields);
    }
}