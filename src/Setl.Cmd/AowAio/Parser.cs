using System.Text.RegularExpressions;
using Setl.Text;

namespace Setl.Cmd.AowAio;

public static partial class Parser
{
    private const string IsBerichtregelPattern = $"^{RecordCodes.BER}";
    private const string IsGemeenteregelPattern = $"^{RecordCodes.GEM}";
    private const string IsDetailregelPattern = $"^{RecordCodes.DTR}";
    private const string IsTellingenregelPattern = $"^{RecordCodes.TPG}";

    private static readonly Regex IsBerichtregel = Parser.IsBerichtregelRegex();
    private static readonly Regex IsGemeenteregel = Parser.IsGemeenteregelRegex();
    private static readonly Regex IsDetailregel = Parser.IsDetailregelRegex();
    private static readonly Regex IsTellingenregel = Parser.IsTellingenregelRegex();

    private static readonly IDictionary<Regex, ITextDeserializer> deserializers =
        new Dictionary<Regex, ITextDeserializer>
        {
            [Parser.IsBerichtregel] = Deserializers.BER,
            [Parser.IsGemeenteregel] = Deserializers.GEM,
            [Parser.IsDetailregel] = Deserializers.DTR,
            [Parser.IsTellingenregel] = Deserializers.TPG,
        };

    public static bool TryParse(string text, out Row row)
    {
        row = new Row();
        
        foreach (var (pattern, deserializer) in Parser.deserializers)
        {
            if (pattern.IsMatch(text))
            {
                row = deserializer.Deserialize(text);
                return true;
            }
        }

        return false;
    }

    [GeneratedRegex(Parser.IsBerichtregelPattern)]
    private static partial Regex IsBerichtregelRegex();
    
    [GeneratedRegex(Parser.IsGemeenteregelPattern)]
    private static partial Regex IsGemeenteregelRegex();
    
    [GeneratedRegex(Parser.IsDetailregelPattern)]
    private static partial Regex IsDetailregelRegex();
    
    [GeneratedRegex(Parser.IsTellingenregelPattern)]
    private static partial Regex IsTellingenregelRegex();
}