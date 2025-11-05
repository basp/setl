namespace Sandbox.Validation;

internal class TellingenValidator : DataValidator
{
    private readonly IValidationErrorHandlers handlers;

    public TellingenValidator(IValidationErrorHandlers handlers)
    {
        this.handlers = handlers;   
    }
    
    public override bool Validate(Line line, Dictionary<string, string> data)
    {
        var valid = true;

        if (!this.ValidateNumeric(data[KnownFields.Gemeentecode]))
        {
            valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnGemeentecodeNietNumeriek(context);       
        }

        if (!this.ValidateNumeric(data[KnownFields.TotaalAantalGerechtigden]))
        {
            valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnTotaalAantalGerechtigdenNietNumeriek(context);       
        }

        if (!this.ValidateNumeric(data[KnownFields.TotaalAantalHuishoudens]))
        {
            valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnTotaalAantalHuishoudensNietNumeriek(context);      
        }

        if (!this.ValidateNumeric(data[KnownFields.TotaalWwbBedrag]))
        {
            valid = false;
            var context = new ValidationErrorContext(line, data);
            this.handlers.OnTotaalWwbBedragNietNumeriek(context);      
        }

        return valid;
    }
}