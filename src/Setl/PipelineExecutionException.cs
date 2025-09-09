namespace Setl;

public class PipelineExecutionException : Exception
{
    internal PipelineExecutionException(
        string message, 
        Exception innerException)
        : base(message, innerException)
    {
    }
}