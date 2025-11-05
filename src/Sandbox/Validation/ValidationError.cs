namespace Sandbox.Validation;

internal class ValidationError
{
    public ValidationError(
        ValidationErrorContext context, 
        string message)
    {
        this.Context = context;
        this.Message = message;
    }
    
    public ValidationErrorContext Context { get; }

    public string Message { get; }
}