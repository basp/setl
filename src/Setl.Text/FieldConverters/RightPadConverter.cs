namespace Setl.Text.FieldConverters;

public class RightPadConverter : IFieldConverter
{
    private readonly char padChar;
    private readonly int length;
    
    public RightPadConverter(char padChar, int length)
    {
        this.padChar = padChar;
        this.length = length;
    }
    
    public bool TryConvert(string value, out object? result)
    {
        result = value.PadRight(this.length, this.padChar);
        return true;
    }

    public string FormatErrorMessage(string name, string value)
    {
        throw new NotImplementedException();
    }
}