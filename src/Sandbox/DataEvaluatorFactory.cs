namespace Sandbox;

internal static class DataEvaluatorFactory
{
    public static IDataEvaluator CreateBerichtEvaluator() =>
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
            },
            new Dictionary<string, Func<object?>>
            {
                ["__TILT__"] = () => "foobar",
            });
}