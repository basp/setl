namespace Setl.Text.FieldConverters;

public class LeftPadConverter : IFieldConverter
{
    private readonly char padChar;
    private readonly int length;
    
    public LeftPadConverter(char padChar, int length)
    {
        this.padChar = padChar;
        this.length = length;
    }

    public bool TryConvert(string value, out object? result)
    {
        result = value.PadLeft(this.length, this.padChar);
        return true;
    }

    public string FormatErrorMessage(string name, string value)
    {
        throw new NotImplementedException();
    }
}