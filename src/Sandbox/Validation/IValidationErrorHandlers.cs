namespace Sandbox.Validation;

internal interface IValidationErrorHandlers
{
    void OnFunctieVersieNietNumeriek(ValidationErrorContext context);
    
    void OnCodeSectorLeverancierNietNumeriek(ValidationErrorContext context);
    
    void OnCodeSectorAanvragerNietNumeriek(ValidationErrorContext context);
    
    void OnDatumAanmaakBerichtNietGeldig(ValidationErrorContext context);
    
    void OnGemeentecodeNietNumeriek(ValidationErrorContext context);
    
    void OnVerwerkingsjaarNietNumeriek(ValidationErrorContext context);
    
    void OnVerwerkingsmaandNietNumeriek(ValidationErrorContext context);
    
    void OnBsnHpNietNumeriek(ValidationErrorContext context);
    
    void OnBsnHpNiet11Proef(ValidationErrorContext context);
    
    void OnGeboortedatumHpNietGeldig(ValidationErrorContext context);
    
    void OnWwbBedragHpNietNumeriek(ValidationErrorContext context);
    
    void OnPostcodeNumeriekNietNumeriek(ValidationErrorContext context);
    
    void OnHuisnummerNietNumeriek(ValidationErrorContext context);
    
    void OnBsnPNietNumeriek(ValidationErrorContext context);
    
    void OnBsnPNiet11Proef(ValidationErrorContext context);
    
    void OnGeboortedatumPNietGeldig(ValidationErrorContext context);
    
    void OnWwbBedragPNietNumeriek(ValidationErrorContext context);
    
    void OnIngangsdatumRechtNietGeldig(ValidationErrorContext context);
    
    void OnEinddatumRechtNietGeldig(ValidationErrorContext context);
    
    void OnTotaalAantalGerechtigdenNietNumeriek(ValidationErrorContext context);
    
    void OnTotaalAantalHuishoudensNietNumeriek(ValidationErrorContext context);
    
    void OnTotaalWwbBedragNietNumeriek(ValidationErrorContext context);
}