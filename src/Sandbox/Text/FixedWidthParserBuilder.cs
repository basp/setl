using System.Text;
using System.Text.RegularExpressions;

namespace Sandbox.Text;

internal class FixedWidthParserBuilder
{
    private readonly List<ISegmentConfig> configs = [];
    
    private bool autoTrim = true;

    private Action<string> onParseError = s =>
        throw new FixedWidthParserException("Invalid input", s); 

    public FixedWidthParserBuilder Field(string name, int length)
    {
        var config = new NamedSegmentConfig(name, length);
        this.configs.Add(config);
        return this;
    }

    public FixedWidthParserBuilder Skip(int length)
    {
        var config = new SkippedSegmentConfig(length);
        this.configs.Add(config);
        return this;
    }

    public FixedWidthParserBuilder AutoTrim(bool value)
    {
        this.autoTrim = value;
        return this;
    }

    public FixedWidthParserBuilder OnParseError(
        Action<string> action)
    {
        this.onParseError = action;
        return this;
    }

    public FixedWidthParser Build()
    {
        var regex = this.BuildRegex();
        return new FixedWidthParser(
            regex, 
            this.configs, 
            this.autoTrim,
            this.onParseError);
    }

    private Regex BuildRegex()
    {
        var patternBuilder = new StringBuilder();
        foreach (var config in this.configs)
        {
            // var fieldPattern = field.Skip
            //     // If we are skipping the field, we don't need to capture it
            //     // in a named group. Just use an unnamed group.
            //     ? $"(.{{{field.Length}}})"
            //     // Otherwise, capture it in a named group.
            //     : $"(?<{field.Name}>.{{{field.Length}}})";
            
            var section = config.GetPattern();
            patternBuilder.Append(section);
        }
        
        var pattern = patternBuilder.ToString();
        return new Regex(pattern);        
    }
}