namespace Setl.Text;

public interface IFieldConverter
{
    bool TryConvert(string value, out object? result);
}