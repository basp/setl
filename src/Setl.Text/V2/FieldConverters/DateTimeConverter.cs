using System.Globalization;

namespace Setl.Text.V2.FieldConverters;

public class DateTimeConverter : IFieldConverter
{
    private readonly string format;
    
    public DateTimeConverter(string format)
    {
        this.format = format;
    }
    
    public DateTimeStyles DateTimeStyles { get; set; } = DateTimeStyles.None;
    
    public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;
    
    public bool TryConvert(string value, out object? result)
    {
        var ok = DateTime.TryParseExact(
            value, 
            this.format, 
            this.CultureInfo, 
            this.DateTimeStyles,
            out var maybe);
        result = maybe;
        return ok;
    }

    public string FormatErrorMessage(string name, string value) =>
        $"{name}: could not convert {value} to a DateTime using format {this.format}"; 
}