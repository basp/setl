namespace Sandbox.Validation;

internal class GemeenteValidator : DataValidator
{
    private readonly IValidationErrorHandlers errorHandlers;

    public GemeenteValidator(IValidationErrorHandlers errorHandlers)
    {
        this.errorHandlers = errorHandlers;
    }

    public override bool Validate(Line line, Dictionary<string, string> data)
    {
        var valid = true;
		
        if (!this.ValidateNumeric(data[KnownFields.Gemeentecode]))
        {
            valid = false;
            this.errorHandlers.OnGemeentecodeNietNumeriek(
                new ValidationErrorContext(line, data));
        }
		
        if (!this.ValidateNumeric(data[KnownFields.Verwerkingsjaar]))
        {
            valid = false;
            this.errorHandlers.OnVerwerkingsjaarNietNumeriek(
                new ValidationErrorContext(line, data));
        }
		
        if (!this.ValidateNumeric(data[KnownFields.Verwerkingsmaand]))
        {
            valid = false;
            this.errorHandlers.OnVerwerkingsmaandNietNumeriek(
                new ValidationErrorContext(line, data));
        }
		
        return valid;
    }
}