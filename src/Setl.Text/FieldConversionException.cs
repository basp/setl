namespace Setl.Text;

public class FieldConversionException : Exception
{
    public FieldConversionException(
        string fieldName,
        string text)
        : base($"Could not convert string '{text}' to field '{fieldName}'")
    {
        this.FieldName = fieldName;
        this.Text = text;
    }
    
    public string FieldName { get; init; }
    
    public string Text { get; init; }
}