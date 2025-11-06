using System.Runtime.InteropServices.JavaScript;

namespace Sandbox.Validation;

internal class ThrowingValidationErrorHandlers : IValidationErrorHandlers
{
    public virtual void OnFunctieVersieNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.FunctieVersieNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnCodeSectorLeverancierNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.CodeSectorLeverancierNietNumeriek);
        throw new ValidationException(error);       
    }

    public virtual void OnCodeSectorAanvragerNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.CodeSectorAanvragerNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnDatumAanmaakBerichtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.DatumAanmaakBerichtNietGeldig);
        throw new ValidationException(error);
    }

    public virtual void OnGemeentecodeNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.GemeentecodeNietNumeriek);
        throw new ValidationException(error);   
    }

    public virtual void OnVerwerkingsjaarNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.VerwerkingsjaarNietNumeriek);
        throw new ValidationException(error);  
    }

    public virtual void OnVerwerkingsmaandNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.VerwerkingsmaandNietNumeriek);
        throw new ValidationException(error); 
    }

    public virtual void OnBsnHpNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.BsnHpNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnBsnHpNiet11Proef(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.BsnHpNiet11Proef);
        throw new ValidationException(error);
    }

    public virtual void OnGeboortedatumHpNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.GeboortedatumHpNietGeldig);
        throw new ValidationException(error);
    }

    public virtual void OnWwbBedragHpNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.WwbBedragHpNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnPostcodeNumeriekNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.PostcodeNumeriekNietNumeriek);
        throw new ValidationException(error);   
    }

    public virtual void OnHuisnummerNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.HuisnummerNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnBsnPNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.BsnPartnerNietNumeriek);
        throw new ValidationException(error);   
    }

    public virtual void OnBsnPNiet11Proef(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.BsnPartnerNiet11Proef);
        throw new ValidationException(error);
    }

    public virtual void OnGeboortedatumPNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.GeboortedatumPartnerNietGeldig);
        throw new ValidationException(error);
    }

    public virtual void OnWwbBedragPNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.WwbBedragPartnerNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnIngangsdatumRechtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.IngangsdatumRechtNietGeldig);
        throw new ValidationException(error);  
    }

    public virtual void OnEinddatumRechtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.EinddatumRechtNietGeldig);
        throw new ValidationException(error); 
    }

    public virtual void OnTotaalAantalGerechtigdenNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.TotaalAantalGerechtigdenNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnTotaalAantalHuishoudensNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.TotaalAantalHuishoudensNietNumeriek);
        throw new ValidationException(error);
    }

    public virtual void OnTotaalWwbBedragNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.TotaalWwbBedragNietNumeriek);
        throw new ValidationException(error);
    }
}