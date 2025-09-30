namespace Setl.Text;

public class RawFieldConverter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = value;
        return true;
    }
}