// ReSharper disable ClassWithVirtualMembersNeverInherited.Global
namespace Sandbox.Validation;

internal class AccumulatingValidationErrorHandlers : IValidationErrorHandlers
{
    private readonly List<ValidationError> errors = [];
    
    public IReadOnlyList<ValidationError> Errors => 
        this.errors;
    
    public virtual void OnFunctieVersieNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.FunctieVersieNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnCodeSectorLeverancierNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context, 
            ErrorMessages.CodeSectorLeverancierNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnCodeSectorAanvragerNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.CodeSectorAanvragerNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnDatumAanmaakBerichtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.DatumAanmaakBerichtNietGeldig);
        this.errors.Add(error);
    }

    public virtual void OnGemeentecodeNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.GemeentecodeNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnVerwerkingsjaarNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.VerwerkingsjaarNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnVerwerkingsmaandNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.VerwerkingsmaandNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnBsnHpNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.BsnHpNietNumeriek);
        this.errors.Add(error);   
    }

    public virtual void OnBsnHpNiet11Proef(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.BsnHpNiet11Proef);
        this.errors.Add(error);  
    }

    public virtual void OnGeboortedatumHpNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.GeboortedatumHpNietGeldig);
        this.errors.Add(error); 
    }

    public virtual void OnWwbBedragHpNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.WwbBedragHpNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnPostcodeNumeriekNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.PostcodeNumeriekNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnHuisnummerNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.HuisnummerNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnBsnPNietNumeriek(ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.BsnPartnerNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnBsnPNiet11Proef(ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.BsnPartnerNiet11Proef);
        this.errors.Add(error); 
    }

    public virtual void OnGeboortedatumPNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.GeboortedatumPartnerNietGeldig);
        this.errors.Add(error);
    }

    public virtual void OnWwbBedragPNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.WwbBedragPartnerNietNumeriek);
        this.errors.Add(error); 
    }

    public virtual void OnIngangsdatumRechtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.IngangsdatumRechtNietGeldig);
        this.errors.Add(error);
    }

    public virtual void OnEinddatumRechtNietGeldig(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.EinddatumRechtNietGeldig);
        this.errors.Add(error);
    }

    public virtual void OnTotaalAantalGerechtigdenNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.TotaalAantalGerechtigdenNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnTotaalAantalHuishoudensNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.TotaalAantalHuishoudensNietNumeriek);
        this.errors.Add(error);
    }

    public virtual void OnTotaalWwbBedragNietNumeriek(
        ValidationErrorContext context)
    {
        var error = new ValidationError(
            context,
            ErrorMessages.TotaalWwbBedragNietNumeriek);
        this.errors.Add(error);
    }
}