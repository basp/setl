namespace Setl.Text;

public class FailingFieldConverter : IFieldConverter
{
    public bool TryConvert(string value, out object? result)
    {
        result = null;
        return false;
    }
}