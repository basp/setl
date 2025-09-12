namespace Setl;

public class Key
{
    private readonly object?[] values;

    public Key(params object?[] values)
    {
        this.values = values;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj is not Key other)
        {
            return false;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (this.values.Length != other.values.Length)
        {
            return false;
        }

        return this.values.SequenceEqual(other.values);
    }

    public override int GetHashCode()
    {
        var result = 0;
        foreach (var value in this.values)
        {
            if (value == null)
            {
                continue;
            }
            
            result ^= value?.GetHashCode() ?? 0;
        }

        return result;
    }
}