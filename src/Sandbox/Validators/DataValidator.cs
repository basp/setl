using System.Globalization;

namespace Sandbox.Validators;

internal abstract class DataValidator
{
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