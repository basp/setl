namespace Setl.Text;

public interface IFieldValidator
{
    bool Validate(string value);

    string FormatErrorMessage(string name, string value);
}