using System.Text.RegularExpressions;

namespace Sandbox.Parsing;

internal class LineParser
{
    private static class MatchGroups
    {
        public const string Code = "Code";
        public const string Data = "Data";
    }
    
    public Action<int, string> OnInvalidLine { get; init; } = (_, _) => {};

    private const string Pattern = "^(?<Code>.{4})(?<Data>.*)$";
    
    private static readonly Regex Regex = 
        new(LineParser.Pattern, RegexOptions.Compiled);
    
    public IEnumerable<Line> Parse(IEnumerable<string> lines)
    {
        var index = 0;
        foreach (var line in lines)
        {
            index += 1;

            // Whitespace is generally fine, but the regex
            // match below will blow up on `null` values.
            if (string.IsNullOrWhiteSpace(line))
            {
                this.OnInvalidLine(index, line);
                continue;
            }

            var match = LineParser.Regex.Match(line);
            if (match.Success)
            {
                var code = match
                    .Groups[MatchGroups.Code]
                    .Value
                    .Trim();
                
                var data = match
                    .Groups[MatchGroups.Data]
                    .Value
                    .Trim();
                
                yield return new Line(index, code, data, line);
                continue;
            }
            
            // If no match (highly unlikely) then we'll just
            // report it as an invalid line.
            this.OnInvalidLine(index, line);
        }
    }
}