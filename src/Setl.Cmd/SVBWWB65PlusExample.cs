using System.Text.Json;
using Setl;

namespace Setl.Cmd;

internal static class SVBWWB65PlusExample
{
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
        
        const string line5 = 
            "TPG 03446          5                    0";
        
        var lines = new[]
        {
            line1, 
            line2, 
            line3, 
            line4, 
            line5,
        };
        
        foreach (var line in lines)
        {
            dynamic tag = new Row();
            tag.Foo = DateTime.Now;
            tag.Bar = Guid.NewGuid();

            dynamic row = WWB65Plus.Parser.Parse(line);
            row.Tag = tag;
            
            WriteRow(row);
        }
    }

    private static JsonSerializerOptions JsonSerializerOptions => new()
    {
        WriteIndented = true,
    };
    
    private static void WriteRow(Row row)
    {
        var json = JsonSerializer.Serialize(
            row, 
            SVBWWB65PlusExample.JsonSerializerOptions);

        Console.WriteLine(json);
    }
}