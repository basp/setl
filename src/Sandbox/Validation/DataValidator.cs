using System.Globalization;

namespace Sandbox.Validation;

internal abstract class DataValidator
{
    protected void ReportValidationError(
        Action<ValidationErrorContext> handler,
        Line line,
        Dictionary<string, string> data)
    {
        var context = new ValidationErrorContext(line, data);
        handler(context);
    }
	
    public abstract bool Validate(
        Line line,
        Dictionary<string, string> data);
	
    protected virtual bool ValidateDate(string value, string format) =>
        DateTime.TryParseExact(
            value.Trim(), 
            format, 
            CultureInfo.InvariantCulture, 
            DateTimeStyles.None, 
            out _);
		
    protected virtual bool ValidateNumeric(string value) =>
        Decimal.TryParse(value, out _);
		
    protected virtual bool ValidateNonEmptyString(string value) =>
        !string.IsNullOrWhiteSpace(value);

    protected virtual bool Validate11Proef(string value) => true;
}