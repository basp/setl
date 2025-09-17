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
    
    public TextSerializer2Builder Skip(int length)
    {
        var name = this.fields.Count.ToString();
        return this.Field(f =>
        {
            f.Name = name;
            f.Length = length;
            f.Skip = true;
        });
    }
    
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
            var fieldPattern = field.Skip
                // If we are skipping the field, we don't need to capture it
                // in a named group. Just use an unnamed group.
                ? $"(.{{{field.Length}}})"
                // Otherwise, capture it in a named group.
                : $"(?<{field.Name}>.{{{field.Length}}})";
            
            patternBuilder.Append(fieldPattern);
        }
        
        var pattern = patternBuilder.ToString();
        var regex = new Regex(pattern);
        return new TextSerializer2(regex, this.fields);
    }
}