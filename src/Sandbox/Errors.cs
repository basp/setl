namespace Sandbox;

internal record Error(int ErrorCode, string Message);

internal static class Errors
{
    // Specifieke validatiefouten (veldniveau)
    public static readonly Error FunctieVersieNietNumeriek =
        new(1100, "FunctieVersieNietNumeriek");
    
    public static readonly Error CodeSectorAanvragerNietNumeriek =
        new(1101, "CodeSectorAanvragerNietNumeriek");
    
    public static readonly Error CodeSectorLeverancierNietNumeriek =
        new(1102, "CodeSectorLeverancierNietNumeriek");
    
    public static readonly Error DatumAanmaakBerichtNietGeldig =
        new(1001, "DatumAanmaakBerichtNietGeldig");

    public static readonly Error GemeentecodeNietNumeriek =
        new(1002, "GemeentecodeNietNumeriek");

    public static readonly Error VerwerkingsjaarNietNumeriek =
        new(1003, "VerwerkingsjaarNietNumeriek");

    public static readonly Error VerwerkingsmaandNietNumeriek =
        new(1004, "VerwerkingsmaandNietNumeriek");

    public static readonly Error BsnHpNietNumeriek =
        new(1005, "BsnHpNietNumeriek");

    public static readonly Error BsnHpNiet11Proef =
        new(1006, "BsnHpNiet11Proef");

    public static readonly Error GeboortedatumHpNietGeldig =
        new(1007, "GeboortedatumHpNietGeldig");

    public static readonly Error WwbBedragHpNietNumeriek =
        new(1008, "WwbBedragHpNietNumeriek");

    public static readonly Error PostcodeNumeriekNietNumeriek =
        new(1009, "PostcodeNumeriekNietNumeriek");

    public static readonly Error HuisnummerNietNumeriek =
        new(1010, "HuisnummerNietNumeriek");

    public static readonly Error BsnPartnerNietNumeriek =
        new(1011, "BsnPartnerNietNumeriek");

    public static readonly Error BsnPartnerNiet11Proef =
        new(1012, "BsnPartnerNiet11Proef");

    public static readonly Error GeboortedatumPartnerNietGeldig =
        new(1013, "GeboortedatumPartnerNietGeldig");

    public static readonly Error WwbBedragPartnerNietNumeriek =
        new(1014, "WwbBedragPartnerNietNumeriek");

    public static readonly Error IngangsdatumRechtNietGeldig =
        new(1015, "IngangsdatumRechtNietGeldig");

    public static readonly Error EinddatumRechtNietGeldig =
        new(1016, "EinddatumRechtNietGeldig");

    public static readonly Error TotaalAantalGerechtigdenNietNumeriek =
        new(1017, "TotaalAantalGerechtigdenNietNumeriek");

    public static readonly Error TotaalAantalHuishoudensNietNumeriek =
        new(1018, "TotaalAantalHuishoudensNietNumeriek");

    public static readonly Error TotaalWwbBedragNietNumeriek =
        new(1019, "TotaalWwbBedragNietNumeriek");

    // Algemene verwerkingsfouten (regelniveau)
    public static readonly Error OnbekendeParser =
        new(2001, "OnbekendeParser");

    public static readonly Error OngeldigeRegel =
        new(2002, "OngeldigeRegel");

    public static readonly Error OnbekendeValidatie =
        new(2003, "OnbekendeValidatie");

    public static readonly Error OnbekendeEvaluatie =
        new(2004, "OnbekendeEvaluatie");

    public static readonly Error MislukteEvaluatie =
        new(2005, "MislukteEvaluatie");

    public static readonly Error OnbekendeConversie =
        new(2006, "OnbekendeConversie");

    public static readonly Error MislukteConversie =
        new(2007, "MislukteConversie");
}