namespace Setl.Text.V1;

public class TrimConverter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = value.Trim();
        return true;
    }   
}