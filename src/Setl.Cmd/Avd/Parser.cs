using System.Text.RegularExpressions;

namespace Setl.Cmd.Avd;

public partial class Parser
{
    public Action<int, string> OnHeaderParseError { get; init; } = (_, _) => {};
    
    public Action<int, string> OnRecordParseError { get; init; } = (_, _) => {};
	
    public IEnumerable<Row> Parse(StreamReader reader)
    {
        var index = 0;
        while(!reader.EndOfStream)
        {
            var line = reader.ReadLine() ?? string.Empty;

            Row row;
            if (index == 0)
            {
                if (!TryMatchHeader(line, out row))
                {
                    this.OnHeaderParseError(index, line);
                    row[FieldNames.Flagged] = true;
                }
            }
            else
            {
                if (!TryMatchRecord(line, out row))
                {
                    this.OnRecordParseError(index, line);
                    row[FieldNames.Flagged] = true;
                }
            }

            row[FieldNames.Index] = index;
            row[FieldNames.Source] = line;
            index += 1;			
            yield return row;
        }
    }
	
    private static bool TryMatchRecord(string line, out Row row)
    {
        row = new Row
        {
            [FieldNames.LineType] = LineType.Record,
            [FieldNames.Bsn] = string.Empty,
            [FieldNames.Gemeentecode] = string.Empty,
        };

        var match = Parser.RecordExpression().Match(line);
        if (!match.Success)
        {			
            return false;
        }

        row = new Row(row)
        {
            [FieldNames.Bsn] = 
                match.Groups[FieldNames.Bsn].Value,
            [FieldNames.Gemeentecode] = 
                match.Groups[FieldNames.Gemeentecode].Value,
        };

        return true;
    }

    private static bool TryMatchHeader(string line, out Row row)
    {
        row = new Row
        {
            [FieldNames.LineType] = LineType.Header,
            [FieldNames.Field0] = string.Empty,
            [FieldNames.Field1]	 = string.Empty,
        };

        var match = Parser.HeaderExpression().Match(line);
        if (!match.Success)
        {			
            return false;
        }

        row = new Row(row)
        {
            [FieldNames.Field0] = 
                match.Groups[FieldNames.Field0].Value,
            [FieldNames.Field1] = 
                match.Groups[FieldNames.Field1].Value,
        };

        return true;
    }

    private static class MatchPatterns
    {
        public const string Header = @"^(?<Field0>BSN);(?<Field1>Gemeentecode)$";
        public const string Record = @"^(?<Bsn>\d{0,9}.)?;(?<Gemeentecode>\d{0,4})$";
    }

    [GeneratedRegex(MatchPatterns.Record)]
    private static partial Regex RecordExpression();
    
    [GeneratedRegex(MatchPatterns.Header)]
    private static partial Regex HeaderExpression();
}