namespace Setl.Text.V1;

public class Int32Converter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        var ok = int.TryParse(value, out var intValue);
        result = intValue;
        return ok;
    }
}