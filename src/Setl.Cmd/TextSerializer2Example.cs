using System.Text.RegularExpressions;
using Setl.Utils;

internal static class TextSerializer2Example
{
 
    private static readonly ITextSerializer berSerializer = 
        new TextSerializer2Builder()
            .Field(f =>
            {
                f.Name = "Recordcode";
                f.Length = 4;
                f.Converter = new TrimConverter();
            })
            .Field(f =>
            {
                f.Name = "Berichttype";
                f.Length = 3;
                f.Converter = new TrimConverter();
            })
            .Build();
    
    private static readonly ITextSerializer gemSerializer =
        new TextSerializer2Builder()
            .Field(f =>
            {
                f.Name = "Recordcode";
                f.Length = 4;
                f.Converter = new TrimConverter();
            })
            .Field(f =>
            {
                f.Name = "Gemeentecode";
                f.Length = 4;
                f.Converter = new PadLeftConverter(4, '0');
            })
            .Build();
    
    private static readonly ITextSerializer dtrSerializer =
        new TextSerializer2Builder()
            .Field(f =>
            {
                f.Name = "Recordcode";
                f.Length = 4;
                f.Converter = new TrimConverter();
            })
            .Field(f =>
            {
                f.Name = "SofinummerHp";
                f.Length = 9;
                f.Converter = new PadLeftConverter(9, '0');
            })
            .Build();
    
    private static readonly ITextSerializer tpgSerializer =
        new TextSerializer2Builder()
            .Field(f =>
            {
                f.Name = "Recordcode";
                f.Length = 4;
                f.Converter = new TrimConverter();
            })
            .Field(f =>
            {
                f.Name = "TotaalAantalGerechtigden";
                f.Length = 11;
                f.Converter = new Int32Converter();
            })
            .Build();
    
    public static void Run()
    {
        const string path = @"D:\temp\SVB\SVBWWB65PLUS00002_3.txt";
        var serializers = new Dictionary<Regex, ITextSerializer>
        {
            [new Regex(@"^BER") ] = TextSerializer2Example.berSerializer,
            [new Regex(@"^GEM") ] = TextSerializer2Example.gemSerializer,
            [new Regex(@"^DTR") ] = TextSerializer2Example.dtrSerializer,
            [new Regex(@"^TPG") ] = TextSerializer2Example.tpgSerializer,
        };

        using var stream = File.OpenRead(path);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            foreach (var serializer in serializers)
            {
                if (!serializer.Key.IsMatch(line))
                {
                    continue;
                }
             
                var row = serializer.Value.Deserialize(line);
                var json = row.ToJson();
                Console.WriteLine(json);
            }
        }
    }
}