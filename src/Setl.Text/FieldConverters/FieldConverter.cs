namespace Setl.Text.FieldConverters;

public class FieldConverter : IFieldConverter
{
    public Func<string, (bool, object)> TryConvert { get; set; } =
        value => (true, value);
    
    public Func<string, object> Convert { get; set; } =
        value => value;
    
    public Func<string, object, string> FormatErrorMessage { get; set; } = 
        (name, value) => $"Invalid value for {name}: {value}";

    bool IFieldConverter.TryConvert(string value, out object? result)
    {
        var (ok, converted) = this.TryConvert(value);
        result = converted;
        return ok;   
    }

    string IFieldConverter.FormatErrorMessage(string name, string value) =>
        this.FormatErrorMessage(name, value);   
}