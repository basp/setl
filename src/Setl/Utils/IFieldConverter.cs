namespace Setl.Utils;

public interface IFieldConverter
{
    bool TryConvert(string value, out object? result);
}