using System.Text.RegularExpressions;
using Setl.Text;
using Setl.Text.V1;

namespace Setl.Cmd.Examples;

internal static class WWB65Plus
{
    private static readonly ITextDeserializer berichtSerializer =
        new SequentialTextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Berichttype", 3)
            .Field("FunctieVersie", 3)
            .Field("NaamBericht", 35)
            .Field("CodeSectorLeverancier", 4)
            .Field("CodeSectorAanvrager", 4)
            .Field("DatumAanmaakBericht", 8)
            .Field("ReferentieLevering", 10)
            .Build();

    private static readonly ITextDeserializer gemeenteSerializer =
        new SequentialTextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("Verwerkingsjaar", 4)
            .Field("Verwerkingsmaand", 2)
            .Build();

    private static readonly ITextDeserializer detailSerializer =
        new SequentialTextDeserializerBuilder()
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

    private static readonly ITextDeserializer tellingenSerializer =
        new SequentialTextDeserializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("TotaalAantalGerechtigden", 11)
            .Field("TotaalAantalHuishoudens", 11)
            .Field("TotaalWWBBedrag", 11)
            .Build();

    private class UnknownRecordException : Exception
    {
        public UnknownRecordException(string text)
            : base("Unknown record type")
        {
            this.Text = text;
        }

        public string Text { get; }
    }
    
    public static class Parser
    {
        // These are the scanner expressions, they tell us what kind of record
        // we're dealing with.
        private static readonly string IsBerichtregelPattern = @"^BER";
        private static readonly string IsGemeenteregelPattern = @"^GEM";
        private static readonly string IsDetailregelPattern = @"^DTR";
        private static readonly string IsTellingenregelPattern = @"^TPG";
        
        // These are the instantiated expressions, they are cached here mostly
        // for performance reasons.
        private static readonly Regex IsBerichtregel = new(Parser.IsBerichtregelPattern);
        private static readonly Regex IsGemeenteregel = new(Parser.IsGemeenteregelPattern);
        private static readonly Regex IsDetailregel = new(Parser.IsDetailregelPattern);
        private static readonly Regex IsTellingenregel = new(Parser.IsTellingenregelPattern);
        
        // This is the serializer config, we configure a serializer for each
        // scanner so it can return a specific record type. Note that records
        // are returned as Row instances. It's up to the caller to cast them
        // to the appropriate type (usually via the ToObject<T> method).
        private static readonly IDictionary<Regex, Func<string, Row>> serializers =
            new Dictionary<Regex, Func<string, Row>>()
            {
                [Parser.IsBerichtregel] = line => 
                    WWB65Plus.berichtSerializer.Deserialize(line),
                [Parser.IsGemeenteregel] = line => 
                    WWB65Plus.gemeenteSerializer.Deserialize(line),
                [Parser.IsDetailregel] = line => 
                    WWB65Plus.detailSerializer.Deserialize(line),
                [Parser.IsTellingenregel] = line => 
                    WWB65Plus.tellingenSerializer.Deserialize(line),
            };
        
        public static Row Parse(string line)
        {
            // Loop through all known serializers and see if we can find a
            // match with the regex key.
            foreach (var (pattern, serializer) in Parser.serializers)
            {
                if (pattern.IsMatch(line))
                {
                    return serializer(line);
                }
            }
            
            throw new UnknownRecordException(line);
        }
    }
}