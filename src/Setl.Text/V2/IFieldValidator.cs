namespace Setl.Text.V2;

public interface IFieldValidator
{
    bool Validate(string value);

    string FormatErrorMessage(string name, string value);
}