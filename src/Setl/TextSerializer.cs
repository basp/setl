using System.Text.RegularExpressions;

namespace Setl;

public class TextSerializer
{
    private readonly Regex regex;
    private readonly string[] fields;
    
    internal TextSerializer(
        Regex regex,
        string[] fields)
    {
        this.regex = regex;
        this.fields = fields;
    }
    
    public T Deserialize<T>(string text) where T : new()
    {
        var match = this.regex.Match(text);
        if (!match.Success)
        {
            const string msg = $"Text does not match the expected format."; 
            throw new ArgumentException(msg);    
        }

        var groups = match.Groups;
        var row = new Row();
        foreach (var field in this.fields)
        {
            // All fields should be matched, otherwise we would have thrown an
            // exception earlier.
            row[field] = groups[field].Value;
        }

        // This could easily fail. Not sure if we should care about it from
        // inside the Deserialize method.
        return row.ToObject<T>();
    }
}