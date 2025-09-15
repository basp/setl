namespace Setl;

public class CompositeKey
{
    private readonly object?[] columnValues;

    public CompositeKey(object?[] columnValues)
    {
        this.columnValues = columnValues;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not CompositeKey other)
        {
            return false;
        }

        if (this.columnValues.Length != other.columnValues.Length)
        {
            return false;
        }

        for (var i = 0; i < this.columnValues.Length; i++)
        {
            var first = this.columnValues[i];
            var second = other.columnValues[i];
            if (!Equals(first, second))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        var result = 0;
        foreach (var value in this.columnValues)
        {
            if (value == null)
            {
                continue;
            }
            
            result = 29 * result + value.GetHashCode();
        }

        return result;
    }
}
