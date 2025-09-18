namespace Setl.Utils;

public class PadRightConverter : IFieldConverter
{
    private readonly int length;
    private readonly char padChar;
    
    public PadRightConverter(int length, char padChar)
    {
        this.length = length;
        this.padChar = padChar;   
    }
    
    public bool TryConvert(string value, out object? result)
    {
        result = value.PadRight(this.length, this.padChar);
        return true;   
    }
}