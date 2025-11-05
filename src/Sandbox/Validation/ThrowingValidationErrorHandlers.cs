namespace Sandbox.Validation;

internal class ThrowingValidationErrorHandlers : IValidationErrorHandlers
{
    public virtual void OnFunctieVersieNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Functie versie moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnCodeSectorLeverancierNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Code sector leverancier moet numeriek zijn";
        throw new ValidationException(message, context);       
    }

    public virtual void OnCodeSectorAanvragerNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Code sector aanvrager moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnDatumAanmaakBerichtNietGeldig(ValidationErrorContext context)
    {
        const string message = "Datum aanmaak bericht moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnGemeentecodeNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Gemeentecode moet numeriek zijn";
        throw new ValidationException(message, context);   
    }

    public virtual void OnVerwerkingsjaarNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Verwerkingsjaar moet numeriek zijn";
        throw new ValidationException(message, context);  
    }

    public virtual void OnVerwerkingsmaandNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Verwerkingsmaand moet numeriek zijn";
        throw new ValidationException(message, context); 
    }

    public virtual void OnBsnHpNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Bsn hoofdpersoon moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnBsnHpNiet11Proef(ValidationErrorContext context)
    {
        const string message = "Bsn hoofdpersoon moet 11-proef zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnGeboortedatumHpNietGeldig(ValidationErrorContext context)
    {
        const string message = "Geboortedatum hoofdpersoon moet geldig zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnWwbBedragHpNietNumeriek(ValidationErrorContext context)
    {
        const string message = "WWB bedrag hoofdpersoon moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnPostcodeNumeriekNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Postcode hoofdpersoon moet numeriek zijn";
        throw new ValidationException(message, context);   
    }

    public virtual void OnHuisnummerNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Huisnummer hoofdpersoon moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnBsnPNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Bsn partner moet numeriek zijn";
        throw new ValidationException(message, context);   
    }

    public virtual void OnBsnPNiet11Proef(ValidationErrorContext context)
    {
        const string message = "Bsn partner moet 11-proef zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnGeboortedatumPNietGeldig(ValidationErrorContext context)
    {
        const string message = "Geboortedatum partner moet geldig zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnWwbBedragPNietNumeriek(ValidationErrorContext context)
    {
        const string message = "WWB bedrag partner moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnIngangsdatumRechtNietGeldig(ValidationErrorContext context)
    {
        const string message = "Ingangsdatum recht moet geldig zijn";
        throw new ValidationException(message, context);  
    }

    public virtual void OnEinddatumRechtNietGeldig(ValidationErrorContext context)
    {
        const string message = "Einddatum recht moet geldig zijn";
        throw new ValidationException(message, context); 
    }

    public virtual void OnTotaalAantalGerechtigdenNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Totaal aantal gerechtigden moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnTotaalAantalHuishoudensNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Totaal aantal huishoudens moet numeriek zijn";
        throw new ValidationException(message, context);
    }

    public virtual void OnTotaalWwbBedragNietNumeriek(ValidationErrorContext context)
    {
        const string message = "Totaal WWB bedrag moet numeriek zijn";
        throw new ValidationException(message, context);
    }
}