namespace Setl;

public class EtlException : Exception
{
    public EtlException(string message, Exception? innerException = null)
    : base(message, innerException)
    {
    }
}