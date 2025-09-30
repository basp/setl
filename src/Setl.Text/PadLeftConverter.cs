namespace Setl.Text;

public class PadLeftConverter : IFieldConverter
{
    private readonly int length;
    private readonly char padChar;

    public PadLeftConverter(int length, char padChar)
    {
        this.length = length;
        this.padChar = padChar;   
    }
    
    public bool TryConvert(string value, out object? result)
    {
        result = value.PadLeft(this.length, this.padChar);
        return true;   
    }
}