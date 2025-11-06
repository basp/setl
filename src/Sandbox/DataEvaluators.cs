namespace Sandbox;

internal static class DataEvaluators
{
    // public static readonly IDataEvaluator BerichtEvaluator =
    //     new MappingDataEvaluator(
    //         new Dictionary<string, Func<string, object?>>
    //         {
    //             [KnownFields.FunctieVersie] = 
    //                 s => int.Parse(s),
    //             [KnownFields.CodeSectorLeverancier] = 
    //                 s => int.Parse(s),
    //             [KnownFields.CodeSectorAanvrager] = 
    //                 s => int.Parse(s),
    //             [KnownFields.DatumAanmaakBericht] = 
    //                 s => DateOnly.ParseExact(s, "yyyyMMdd"),
    //         });
    
    public static readonly IDataEvaluator BerichtEvaluator =
        DataEvaluatorFactory.CreateBerichtEvaluator();

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
                    s => ParseDateOnlyOrValue(
                        s, 
                        "yyyyMMdd", 
                        null),
                [KnownFields.WwbBedragPartner] =
                    s => DataEvaluators.ParseIntOrValue(s, null),
                [KnownFields.IngangsdatumRecht] =
                    s => DateOnly.ParseExact(s, "yyyyMMdd"),
                [KnownFields.EinddatumRecht] =
                    s => DataEvaluators.ParseDateOnlyOrValue(
                        s, 
                        "yyyyMMdd", 
                        null),
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
                [KnownFields.TotaalWwbBedrag] =
                    s => int.Parse(s),
            });
    
    private static object? ParseIntOrValue(string s, object? value) =>
        int.TryParse(s, out var result) 
            ? result 
            : value;

    private static object? ParseDateOnlyOrValue(
        string s, 
        string format, 
        object? value) =>
            DateOnly.TryParseExact(s, format, out var result) 
                ? result 
                : value;
}