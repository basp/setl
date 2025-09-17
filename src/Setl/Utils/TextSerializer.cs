using System.Text.RegularExpressions;

namespace Setl.Utils;

public class TextSerializer : ITextSerializer
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

    public Row Deserialize(string text)
    {
        var match = this.regex.Match(text);
        if (!match.Success)
        {
            const string msg = $"Text does not match the expected format."; 
            throw new ArgumentException(msg);    
        }

        var items = this.fields
            .ToDictionary(
                x => x, 
                object? (x) => match.Groups[x].Value);
        
        return new Row(items);
    }
    
    public T Deserialize<T>(string text) where T : new()
    {
        var row = this.Deserialize(text);
        return row.ToObject<T>();
    }
}