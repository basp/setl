namespace Setl.Text.V1;

public interface IFieldConverter
{
    bool TryConvert(string value, out object? result);
}