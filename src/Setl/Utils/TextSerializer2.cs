using System.Text.RegularExpressions;

namespace Setl.Utils;

public class TextSerializer2 : ITextSerializer
{
    private readonly Regex regex;
    private readonly IList<TextField> fields;

    internal TextSerializer2(
        Regex regex,
        IList<TextField> fields)
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
                x => x.Name,
                x => x.Convert(match.Groups[x.Name].Value));

        return new Row(items);
    }

    public T Deserialize<T>(string text) where T : new()
    {
        var row = this.Deserialize(text);
        return row.ToObject<T>();
    }
}