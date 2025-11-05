using Sandbox.Text;

namespace Sandbox.Parsing;

internal static class DataParsers
{
    internal static readonly IFixedWidthParser BerichtdataParser =
        new FixedWidthParserBuilder()
            .Field("Berichttype", 3)
            .Field("FunctieVersie", 3)
            .Field("NaamBericht", 35)
            .Field("CodeSectorLeverancier", 4)
            .Field("CodeSectorAanvrager", 4)
            .Field("DatumAanmaakBericht", 8)
            .Field("ReferentieLevering", 10)
            .Build();
    
    internal static readonly IFixedWidthParser GemeentedataParser =
        new FixedWidthParserBuilder()
            .Field("Gemeentecode", 4)
            .Field("Verwerkingsjaar", 4)
            .Field("Verwerkingsmaand", 2)
            .Build();

    internal static readonly IFixedWidthParser DetaildataParser =
        new FixedWidthParserBuilder()
            .Field("SofinummerHp", 9)
            .Field("AchternaamHp", 25)
            .Field("VoorlettersHp", 6)
            .Field("VoorvoegselHp", 10)
            .Field("EersteVoornaamHp", 28)
            .Field("GeboortedatumHp", 8)
            .Field("RekeningnummerHp", 40)
            .Field("WWBBedragHp", 9)
            .Field("PostcodeNumeriek", 4)
            .Field("PostcodeLetters", 2)
            .Field("Huisnummer", 5)
            .Field("HuisnummerToevoeging", 4)
            .Field("Straatnaam", 24)
            .Field("Plaatsnaam", 24)
            .Field("SofinummerP", 9)
            .Field("AchternaamP", 25)
            .Field("VoorlettersP", 6)
            .Field("VoorvoegselP", 10)
            .Field("EersteVoornaamP", 28)
            .Field("GeboortedatumP", 8)
            .Field("RekeningnummerP", 40)
            .Field("WWBBedragP", 9)
            .Field("IngangsdatumRecht", 8)
            .Field("EinddatumRecht", 8)
            .Field("WWBNorm", 2)
            .Build();

    internal static readonly IFixedWidthParser TellingdataParser =
        new FixedWidthParserBuilder()
            .Field("Gemeentecode", 4)
            .Field("TotaalAantalGerechtigden", 11)
            .Field("TotaalAantalHuishoudens", 11)
            .Field("TotaalWWBBedrag", 11)
            .Build();
}