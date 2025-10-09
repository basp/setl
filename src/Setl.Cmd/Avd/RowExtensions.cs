namespace Setl.Cmd.Avd;

public static class RowExtensions
{
    public static int GetInt32(
        this Row self,
        string field,
        int missing) =>
        self.TryGetValue(field, out var value) 
            ? Convert.ToInt32(value) 
            : missing;

    public static string GetString(
        this Row self,
        string field,
        string missing)
    {
        if (self.TryGetValue(field, out var value))
        {
            return Convert.ToString(value) ?? string.Empty;
        }
        
        return missing;
    }
    
    public static int GetInt32(this Row self, string field)
    {
        if (self.TryGetValue(field, out var value))
        {
            return Convert.ToInt32(value);
        }

        throw CreateKeyNotFoundException(field);
    }
    
    public static string GetString(this Row self, string field)
    {
        if (self.TryGetValue(field, out var value))
        {
            return Convert.ToString(value) ?? string.Empty;
        }

        throw CreateKeyNotFoundException(field);
    }
    
    public static bool GetBool(this Row self, string field) =>
        Convert.ToBoolean(self[field]);

    public static bool IsLineType(this Row self, LineType expected)
    {
        if (!self.TryGetValue(FieldNames.LineType, out var value))
        {
            return false;
        }
		
        if (Enum.TryParse(
                typeof(LineType), 
                value.ToString(), 
                ignoreCase: true, 
                out var actual))
        {
            return expected == (LineType)actual;
        }
		
        return false;
    }

    public static bool IsFlagged(this Row self) =>
        self.ContainsKey(FieldNames.Flagged) && self.GetBool(FieldNames.Flagged);

    private static Exception CreateKeyNotFoundException(string field)
    {
        var msg = $"Cannot find field '{field}' in row.";
        return new KeyNotFoundException(msg);
    }
}