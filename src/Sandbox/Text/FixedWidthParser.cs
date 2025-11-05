using System.Text.RegularExpressions;

namespace Sandbox.Text;

internal class FixedWidthParser : IFixedWidthParser
{
    private readonly Regex regex;
    private readonly IList<ISegmentConfig> segments;
    private readonly bool autoTrim;
    private readonly Action<string> onParseError;
    
    public FixedWidthParser(
        Regex regex,
        IList<ISegmentConfig> fields,
        bool autoTrim,
        Action<string> onParseError)
    {
        this.regex = regex;
        this.segments = fields;
        this.autoTrim = autoTrim;
        this.onParseError = onParseError;
    }
    
    public Dictionary<string, string> Parse(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            this.onParseError(text);
        }
        
        var match = this.regex.Match(text);
        if (!match.Success)
        {
            this.onParseError(text);
        }
        
        var row = new Dictionary<string, string>();
        foreach (var segment in this.segments)
        {
            // We don't need to include unnamed segments in the result.
            if (!segment.TryGetName(out var field))
            {
                continue;
            }
            
            // Ensure there's a key for this field.
            row.Add(field, string.Empty);

            // Grab the matched value for this field.
            var value = match.Groups[field].Value;

            // Automatically trim any whitespace around the
            // value (if required).
            if (this.autoTrim)
            {
                value = value.Trim();
            }

            row[field] = value;
        }
        
        return row;
    }

    public bool TryParse(string text, out Dictionary<string, string> result)
    {
        try
        {
            result = this.Parse(text);
            return true;
        }
        catch
        {
            result = new Dictionary<string, string>();
        }
        
        return false;
    }
}