namespace Sandbox.Validation;

internal class ValidationException : Exception
{
    public ValidationException(
        string message, 
        ValidationErrorContext context)
        : base(message)
    {
        this.Context = context;
    }
    public ValidationErrorContext Context { get; }
}