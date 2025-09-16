using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Setl;

using var loggerFactory =
    LoggerFactory.Create(builder =>
        builder.AddSimpleConsole(cfg =>
            {
                cfg.TimestampFormat = "HH:mm:ss ";
                cfg.SingleLine = true;
            })
            .SetMinimumLevel(LogLevel.Trace));

// SimpleExample.Run();
SVBWWB65PlusExample.Run();

internal static class SVBWWB65PlusExample
{
    private static readonly ITextSerializer berichtSerializer =
        new SequentialTextSerializerBuilder()
            .Field("Recordcode", 4)
            .Field("Berichttype", 3)
            .Field("FunctieVersie", 3)
            .Field("NaamBericht", 35)
            .Field("CodeSectorLeverancier", 4)
            .Field("CodeSectorAanvrager", 4)
            .Field("DatumAanmaakBericht", 8)
            .Field("ReferentieLevering", 10)
            .Build();

    private static readonly ITextSerializer gemeenteSerializer =
        new SequentialTextSerializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("Verwerkingsjaar", 4)
            .Field("Verwerkingsmaand", 2)
            .Build();

    private static readonly ITextSerializer detailSerializer =
        new SequentialTextSerializerBuilder()
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

    private static ITextSerializer tellingenSerializer =
        new SequentialTextSerializerBuilder()
            .Field("Recordcode", 4)
            .Field("Gemeentecode", 4)
            .Field("TotaalAantalGerechtigden", 11)
            .Field("TotaalAantalHuishoudens", 11)
            .Field("TotaalWWBBedrag", 11)
            .Build();
    
    public static void Run()
    {
        const string line1 = 
            "BER 325001Levering SVB aan IB: WWB 65plus    0035004120250910A123456789   ";
        const string line2 = 
            "GEM 0344202509";
        const string line3 =
            "DTR 166247650Janssen                  A               Arie                        19410515                                             0   2400AG  240    Teststraat              Testdorp                138124498Janhanssen               AQA             Quintus                     19420401SE3550000000054910000003                        0201301012024010301";
        const string line4 =
            "DTR 194228885Boomstra                 F     van       Fred                        19501129NL37INGB0628673523                           0   5555BB  333    Testlaan                Testdorp                                                                                                                                                      0201301010000000002";
        const string line5 = "TPG 03446          5                    0";
        var lines = new[] { line1, line2, line3, line4, line5 };
        foreach (var line in lines)
        {
            var row = Parser.Parse(line);
            WriteRow(row);
        }
    }
    
    private static void WriteRow(Row row)
    {
        var json = JsonSerializer.Serialize(
            row, 
            new JsonSerializerOptions
            {
                WriteIndented = true,
            });

        Console.WriteLine(json);
    }

    private class Parser
    {
        private static readonly string IsBerichtregelPattern = @"^BER";
        private static readonly string IsGemeenteregelPattern = @"^GEM";
        private static readonly string IsDetailregelPattern = @"^DTR";
        private static readonly string IsTellingenregelPattern = @"^TPG";
        
        private static readonly Regex IsBerichtregel = new(IsBerichtregelPattern);
        private static readonly Regex IsGemeenteregel = new(IsGemeenteregelPattern);
        private static readonly Regex IsDetailregel = new(IsDetailregelPattern);
        private static readonly Regex IsTellingenregel = new(IsTellingenregelPattern);
        
        private static readonly IDictionary<Regex, Func<string, Row>> serializers =
            new Dictionary<Regex, Func<string, Row>>()
            {
                [Parser.IsBerichtregel] = line => 
                    SVBWWB65PlusExample.berichtSerializer.Deserialize(line),
                [Parser.IsGemeenteregel] = line => 
                    SVBWWB65PlusExample.gemeenteSerializer.Deserialize(line),
                [Parser.IsDetailregel] = line => 
                    SVBWWB65PlusExample.detailSerializer.Deserialize(line),
                [Parser.IsTellingenregel] = 
                    line => SVBWWB65PlusExample.tellingenSerializer.Deserialize(line),
            };
        
        public static Row Parse(string line)
        {
            foreach (var (pattern, serializer) in serializers)
            {
                if (pattern.IsMatch(line))
                {
                    return serializer(line);
                }
            }
            
            // Include line and other metadata (line number, etc.)
            throw new Exception("Unknown record type");
        }
    }
}

internal static class SimpleExample
{
    public static void Run()
    {
        var builder = new SequentialTextSerializerBuilder()
            .Field("foo", 3)
            .Field("bar", 3)
            .Skip(3)
            .Field("quux", 3);

        const string text = @"123XYZ   123";
        var serializer = builder.Build();

        var record1 = serializer.Deserialize<Record>(text);
        Console.WriteLine(
            "[Foo {0}] [Bar {1}] [Quux {2}]",
            record1.foo,
            record1.bar,
            record1.quux);

        var record2 = serializer.Deserialize<MinimalRecord>(text);
        Console.WriteLine("[Foo {0}]", record2.foo);
    }

    private class MinimalRecord
    {
        public string foo { get; set; } = string.Empty;
    }

    private class Record
    {
        public string foo { get; set; } = string.Empty;
        public string bar { get; set; } = string.Empty;
        public string quux { get; set; } = string.Empty;
    }
}