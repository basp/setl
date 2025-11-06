namespace Sandbox.Support;

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

        if (this.values.Length != other.values.Length)
        {
            return false;
        }

        return this.values.SequenceEqual(other.values);
    }

    public override int GetHashCode()
    {
        var result = 0;

        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var value in this.values)
        {
            if (value == null)
            {
                continue;
            }

            result ^= value.GetHashCode();
        }

        return result;
    }
}