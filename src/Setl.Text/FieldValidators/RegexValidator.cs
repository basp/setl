using System.Text.RegularExpressions;

namespace Setl.Text.FieldValidators;

public class RegexValidator : IFieldValidator
{
    private readonly Regex regex;

    public RegexValidator(Regex regex)
    {
        this.regex = regex;
    }
    
    public RegexValidator(
        string pattern, 
        RegexOptions options = RegexOptions.None)
    {
        this.regex = new Regex(pattern, options);
    }

    public bool Validate(string value) => this.regex.IsMatch(value);

    public string FormatErrorMessage(string name, string value) =>
        $"{name}: {value} did not match the pattern";   
}