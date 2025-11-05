namespace Sandbox.Validation;

internal class SilentValidationErrorHandlers : IValidationErrorHandlers
{
    public virtual void OnFunctieVersieNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnCodeSectorLeverancierNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnCodeSectorAanvragerNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnDatumAanmaakBerichtNietGeldig(
        ValidationErrorContext context)
    {
    }

    public virtual void OnGemeentecodeNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnVerwerkingsjaarNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnVerwerkingsmaandNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnBsnHpNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnBsnHpNiet11Proef(
        ValidationErrorContext context)
    {
    }

    public virtual void OnGeboortedatumHpNietGeldig(
        ValidationErrorContext context)
    {
    }

    public virtual void OnWwbBedragHpNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnPostcodeNumeriekNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnHuisnummerNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnBsnPNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnBsnPNiet11Proef(
        ValidationErrorContext context)
    {
    }

    public virtual void OnGeboortedatumPNietGeldig(
        ValidationErrorContext context)
    {
    }

    public virtual void OnWwbBedragPNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnIngangsdatumRechtNietGeldig(
        ValidationErrorContext context)
    {
    }

    public virtual void OnEinddatumRechtNietGeldig(
        ValidationErrorContext context)
    {
    }

    public virtual void OnTotaalAantalGerechtigdenNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnTotaalAantalHuishoudensNietNumeriek(
        ValidationErrorContext context)
    {
    }

    public virtual void OnTotaalWwbBedragNietNumeriek(
        ValidationErrorContext context)
    {
    }
}