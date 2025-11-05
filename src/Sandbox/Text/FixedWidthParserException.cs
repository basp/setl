namespace Sandbox.Text;

internal class FixedWidthParserException : Exception
{
    public FixedWidthParserException(string message, string text)
        : base(message)
    {
        this.Text = text;
    }
        
    public string Text { get; }
}