using System.Text.RegularExpressions;

namespace Setl.Text.Fixed;

internal class TextDeserializer : ITextDeserializer
{
    private readonly Regex regex;
    private readonly IList<FieldConfiguration> fields;
    private readonly bool autoTrim;
    
    public TextDeserializer(
        Regex regex,
        IList<FieldConfiguration> fields,
        bool autoTrim = true)
    {
        this.regex = regex;
        this.fields = fields;
        this.autoTrim = autoTrim;
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
            // We still to preserve the structure of the row, even though
            // some validations and/or conversions might fail.
            row.Add(field.Name, string.Empty);

            // Grab the matched value for this field.
            var value = match.Groups[field.Name].Value;

            // We assume that the field is valid by default.
            var isValid = true;
            
            // Execute all registered validators for this field.
            foreach (var validator in field.Validators)
            {
                if (!validator.Validate(value))
                {
                    // Failed to validate.
                    validationErrors.Add(
                        validator.FormatErrorMessage(
                            field.Name,
                            value));

                    // Stop after the first validation error for this field
                    // and mark this field as invalid. This will prevent
                    // any conversion shenanigans.
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                // We got at least one validation error for this field,
                // so don't bother to convert it. Just continue with the
                // next field.
                continue;
            }

            // Automatically trim any whitespace around the value.
            if (this.autoTrim)
            {
                value = value.Trim();
            }

            if (field.Converter.TryConvert(value, out var converted))
            {
                // Successfully dealt with this field, carry on with the next.
                row[field.Name] = converted;
                continue;
            }

            // Failed to convert.
            conversionErrors.Add(
                field.Converter.FormatErrorMessage(
                    field.Name,
                    value));
        }

        // For now, we'll just send any errors along inside the row.
        row[WellKnownKeys.ValidationErrorsKey] = validationErrors;
        row[WellKnownKeys.ConversionErrorsKey] = conversionErrors;
        return row;
    }

    public T Deserialize<T>(string text) where T : new() =>
        this.Deserialize(text).ToObject<T>();
}