namespace Setl.Text.V2.FieldConverters;

public class Int32Converter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        var ok = int.TryParse(value, out var maybe);
        result = maybe;
        return ok;
    }

    public string FormatErrorMessage(string name, string value) =>
        $"{name}: could not convert {value} to an Int32.";
}