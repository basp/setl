namespace Setl.Text.FieldConverters;

public class NopFieldConverter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = value;
        return true;
    }

    public string FormatErrorMessage(string name, string value)
    {
        throw new NotImplementedException();
    }
}