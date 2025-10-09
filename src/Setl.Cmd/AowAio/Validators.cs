using Setl.Text;

namespace Setl.Cmd.AowAio;

public static class Validators
{
    public class IsNumericValidator : IFieldValidator
    {
        public bool Validate(string value) =>  decimal.TryParse(value, out _);

        public string FormatErrorMessage(string name, string value) =>
            $"{name}: {value} is not numeric";
    }
}