namespace Setl.Text.FieldConverters;

public class TrimConverter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = value?.Trim();
        return true;
    }

    public string FormatErrorMessage(string name, string value)
    {
        throw new NotImplementedException();
    }
}