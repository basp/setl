using System.Text.RegularExpressions;

namespace Setl.Text.V2.Fixed;

public class TextDeserializer : ITextDeserializer
{
    private readonly Regex regex;
    private readonly IList<FieldConfiguration> fields;
    
    public TextDeserializer(
        Regex regex,
        IList<FieldConfiguration> fields)
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
        
        var validationErrors = new List<string>();
        var conversionErrors = new List<string>();
        var row = new Row();
        
        // NOTE:
        // The reason why we are still including the fields in case of 
        // validation or conversion errors is that we want to keep the
        // structure of the rows coming into the ETL process stable at
        // runtime. This makes it easier to reason about the flow of the
        // data during development time.
        
        foreach (var field in this.fields)
        {
            var value = match.Groups[field.Name].Value;
            if(field.Validator.Validate(value))
            {
                if (field.Converter.TryConvert(value, out var converted))
                {
                    // Successfully dealt with this field.
                    row.Add(field.Name, converted);
                    continue;
                }

                // Failed to convert.
                conversionErrors.Add(
                    field.Converter.FormatErrorMessage(field.Name, value));
                // Even though we failed to convert, we still want the field.
                row.Add(field.Name, string.Empty);
                continue;
            }
        
            // Failed to validate.
            validationErrors.Add(
                field.Validator.FormatErrorMessage(field.Name, value));
            // Again, we still want the field.
            row.Add(field.Name, string.Empty);
        }
        
        // For now, we'll just send any errors along inside the row.
        row["__VALIDATION_ERRORS"] = validationErrors;
        row["__CONVERSION_ERRORS"] = conversionErrors;
        return row;
    }

    public T Deserialize<T>(string text) where T : new() =>
        this.Deserialize(text).ToObject<T>();
}