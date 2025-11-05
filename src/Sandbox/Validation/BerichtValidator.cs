namespace Sandbox.Validation;

internal class BerichtValidator : DataValidator
{
    private readonly IValidationErrorHandlers handlers;
    
    public BerichtValidator(IValidationErrorHandlers handlers)
    {
        this.handlers = handlers;
    }
    
    public override bool Validate(
        Line line, 
        Dictionary<string, string> data)
    {
        var valid = true;

        if (!this.ValidateNumeric(data[KnownFields.FunctieVersie]))
        {
            valid = false;
            this.handlers.OnFunctieVersieNietNumeriek(
                new ValidationErrorContext(line, data));
        }
        
        if (!this.ValidateNumeric(data[KnownFields.CodeSectorLeverancier]))
        {
            valid = false;
            this.handlers.OnCodeSectorLeverancierNietNumeriek(
                new ValidationErrorContext(line, data));
        }
		
        if (!this.ValidateNumeric(data[KnownFields.CodeSectorAanvrager]))
        {
            valid = false;
            this.handlers.OnCodeSectorAanvragerNietNumeriek(
                new ValidationErrorContext(line, data));
        }
		
        if (!this.ValidateDate(data[KnownFields.DatumAanmaakBericht], "yyyyMMdd"))
        {
            valid = false;
            this.handlers.OnDatumAanmaakBerichtNietGeldig(
                new ValidationErrorContext(line, data));
        }

        return valid;
    }
}