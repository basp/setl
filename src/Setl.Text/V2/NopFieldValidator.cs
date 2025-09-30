namespace Setl.Text.V2;

public class NopFieldValidator : IFieldValidator
{
    public bool Validate(string value)
    {
        return true;
    }

    public string FormatErrorMessage(string name, string value)
    {
        throw new NotImplementedException();
    }
}