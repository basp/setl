namespace Setl.Text;

public class TextDeserializationException : Exception
{
    public TextDeserializationException(
        string message,
        string text)
        : base(message)
    {
        this.Text = text;
    }
        
    public string Text { get; }
}