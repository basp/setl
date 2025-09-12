namespace Setl;

public class Row : DynamicDictionary
{
    public Row()
    {
    }

    public Row(IDictionary<string, object?> items)
        : base(items)
    {
    }

    public Row(StringComparer comparer)
        : base(comparer)
    {
    }

    public Row(IDictionary<string, object?> items, StringComparer comparer)
        : base(items, comparer)
    {
    }

    public IEnumerable<string> Columns
    {
        get
        {
            var keys = new List<string>(this.items.Keys);
            foreach (var key in keys)
            {
                yield return key;
            }
        }
    }

    public Row Clone() => new(this.items);

    public Key CreateKey() => this.CreateKey(this.Columns.ToArray());

    public Key CreateKey(params string[] columns)
    {
        var arr = new object?[columns.Length];
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            arr[i] = this.items[column];
        }

        return new Key(arr);
    }
}