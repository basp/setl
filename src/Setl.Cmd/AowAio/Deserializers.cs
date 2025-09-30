using Setl.Text;
using Setl.Text.FieldConverters;
using Setl.Text.Fixed;

namespace Setl.Cmd.AowAio;

public static class Deserializers
{
    public static readonly ITextDeserializer BER =
        new TextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Berichttype", 2)
            .Field("FunctieVersie", 3)
            .Field("NaamBericht", 35)
            .Field("CodeSectorLeverancier", 4)
            .Field("CodeSectorAanvrager", 4)
            .Field("DatumAanmaakBericht", 8)
            .Field("ReferentieLevering", 10)
            .Build();

    public static readonly ITextDeserializer GEM =
        new TextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("Verwerkingsjaar", 4)
            .Field("Verwerkingsmaand", 2)
            .Build();
    
    public static readonly ITextDeserializer DTR =
        new TextDeserializerBuilder()
            .Field("Recordcode", 4)
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
    
    public static readonly ITextDeserializer TPG =
        new TextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("TotaalAantalGerechtigden", 11)
            .Field("TotaalAantalHuishoudens", 11)
            .Field("TotaalWWBBedrag", 11)
            .Build();
}