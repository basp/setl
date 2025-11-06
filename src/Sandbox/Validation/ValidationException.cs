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

    public ValidationException(ValidationError error)
        : this(error.Message, error.Context)
    {
    }
    
    public ValidationErrorContext Context { get; }
}