using System.Text.RegularExpressions;

namespace Setl.Text.V1;

public class TextDeserializer2 : ITextDeserializer
{
    private readonly Regex regex;
    private readonly IList<TextField> fields;

    internal TextDeserializer2(
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
            throw new TextDeserializationException(msg, text);   
        }

        var items = this.fields
            .ToDictionary(
                x => x.Name,
                x =>
                {
                    var value = match.Groups[x.Name].Value;
                    if (!x.Converter.TryConvert(value, out var result))
                    {
                        throw new FieldConversionException(x.Name, value);
                    }

                    return result;
                });

        return new Row(items);
    }

    public T Deserialize<T>(string text) where T : new()
    {
        var row = this.Deserialize(text);
        return row.ToObject<T>();
    }
}