using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Setl.Text.V2;

namespace Setl.Cmd.AowAio;

public class Parser
{
    private const string IsBerichtregelPattern = @"^BER";
    private const string IsGemeenteregelPattern = @"^GEM";
    private const string IsDetailregelPattern = @"^DTR";
    private const string IsTellingenregelPattern = @"^TPG";

    private static readonly Regex IsBerichtregel = new(Parser.IsBerichtregelPattern);
    private static readonly Regex IsGemeenteregel = new(Parser.IsGemeenteregelPattern);
    private static readonly Regex IsDetailregel = new(Parser.IsDetailregelPattern);
    private static readonly Regex IsTellingenregel = new(Parser.IsTellingenregelPattern);

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
}