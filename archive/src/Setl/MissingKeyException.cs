namespace Setl;

public class MissingKeyException : Exception
{
    public MissingKeyException(string message)
        : base($"Could not find key: {message}")
    {
    }

    public MissingKeyException(
        string message,
        Exception innerException)
        : base(message, innerException)
    {
    }
}