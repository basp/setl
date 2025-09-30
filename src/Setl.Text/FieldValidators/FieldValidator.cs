namespace Setl.Text.FieldValidators;

public class FieldValidator : IFieldValidator
{
    public Func<string, bool> Validate { get; set; } = value => true;
    
    public Func<string, string, string> FormatErrorMessage { get; set; } = 
        (name, value) => $"Invalid value for {name}: {value}";

    bool IFieldValidator.Validate(string value) =>
        this.Validate(value);

    string IFieldValidator.FormatErrorMessage(string name, string value) =>
        this.FormatErrorMessage(name, value);
}