namespace Sandbox;

internal static class Evaluators
{
    public static readonly IDataEvaluator BerichtEvaluator =
        new MappingDataEvaluator(
            new Dictionary<string, Func<string, object?>>
            {
                [KnownFields.FunctieVersie] = 
                    s => int.Parse(s),
                [KnownFields.CodeSectorLeverancier] = 
                    s => int.Parse(s),
                [KnownFields.CodeSectorAanvrager] = 
                    s => int.Parse(s),
                [KnownFields.DatumAanmaakBericht] = 
                    s => DateOnly.ParseExact(s, "yyyyMMdd"),
            });

    public static readonly IDataEvaluator GemeenteEvaluator =
        new MappingDataEvaluator(
            new Dictionary<string, Func<string, object?>>
            {
                [KnownFields.Gemeentecode] = 
                    s => int.Parse(s),
                [KnownFields.Verwerkingsjaar] =
                    s => int.Parse(s),
                [KnownFields.Verwerkingsmaand] =
                    s => int.Parse(s),
            });
    
    public static readonly IDataEvaluator DetailEvaluator =
        new MappingDataEvaluator(
            new Dictionary<string, Func<string, object?>>
            {
                [KnownFields.GeboortedatumHoofdpersoon] =
                    s => DateOnly.ParseExact(s, "yyyyMMdd"),
                [KnownFields.WwbBedragHoofdpersoon] =
                    s => int.Parse(s),
                [KnownFields.PostcodeNumeriek] =
                    s => int.Parse(s),
                [KnownFields.Huisnummer] =
                    s => int.Parse(s),
                [KnownFields.GeboortedatumPartner] =
                    s => ParseOptionalDateOnly(s, "yyyyMMdd"),
                [KnownFields.WwbBedragPartner] =
                    Evaluators.ParseOptionalInt,
                [KnownFields.IngangsdatumRecht] =
                    s => DateOnly.ParseExact(s, "yyyyMMdd"),
                [KnownFields.EinddatumRecht] =
                    s => Evaluators.ParseOptionalDateOnly(s, "yyyyMMdd"),
            });


    public static readonly IDataEvaluator TellingdataEvaluator =
        new MappingDataEvaluator(
            new Dictionary<string, Func<string, object?>>
            {
                [KnownFields.Gemeentecode] =
                    s => int.Parse(s),
                [KnownFields.TotaalAantalHuishoudens] = 
                    s => int.Parse(s),
                [KnownFields.TotaalAantalGerechtigden] =
                    s => int.Parse(s),
            });
    
    private static object? ParseOptionalInt(string s)
    {
        if (int.TryParse(s, out var result))
        {
            return result;
        }

        return null;
    }

    private static object? ParseOptionalDateOnly(string s, string format)
    {
        if (DateOnly.TryParseExact(s, format, out var result)) 
        {
            return result;
        }

        return null;
    }
}