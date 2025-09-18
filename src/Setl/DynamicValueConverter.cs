namespace Setl;

public class DynamicValueConverter : IDynamicValueConverter
{
    public static readonly IDynamicValueConverter None =
        new DynamicValueConverter(x => x);

    public static readonly IDynamicValueConverter Database =
        new DynamicValueConverter(x => x == null ? DBNull.Value : null);
    
    private readonly Func<object?, object?> converter;
    
    private DynamicValueConverter(Func<object?, object?> converter)
    {
        this.converter = converter;
    }
    
    public object? Convert(object? value) => this.converter(value);
}