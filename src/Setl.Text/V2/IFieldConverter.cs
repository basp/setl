namespace Setl.Text.V2;

public interface IFieldConverter
{
    bool TryConvert(string value, out object? result);
    
    string FormatErrorMessage(string name, string value);
}