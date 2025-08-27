using System.Dynamic;

namespace Setl;

using System.Collections;
using System.Diagnostics;
using System.Text.Json;

[Serializable]
public class DynamicDictionary 
    : DynamicObject, IDictionary<string, object?>
{
    public static StringComparer DefaultComparer { get; set; }

    static DynamicDictionary()
    {
        DefaultComparer = StringComparer.InvariantCultureIgnoreCase;
    }

    protected IDictionary<string, object?> items;

    protected string lastAccess = string.Empty;

    private bool throwIfMissing;

    protected StringComparer Comparer { get; set; }

    public ICollection<string> Keys => this.items.Keys;

    public ICollection<object?> Values => this.items.Values;

    public int Count => this.items.Count;

    public bool IsReadOnly => this.items.IsReadOnly;

    public DynamicDictionary(
        IDictionary<string, object?> items,
        StringComparer comparer)
    {
        this.Comparer = comparer;
        
        this.items = new Dictionary<string, object?>(items, comparer);
    }

    public DynamicDictionary(IDictionary<string, object?> items)
        : this(items, DefaultComparer)
    {
    }

    public object? this[string key]
    {
        get
        {
            if (this.throwIfMissing && this.items.ContainsKey(key) == false)
            {
                throw new MissingKeyException(key);
            }

            this.lastAccess = key;
            return this.items[key];
        }
        set
        {
            this.lastAccess = key;
            if (value == DBNull.Value)
            {
                this.items[key] = null;
            }
            else
            {
                this.items[key] = value;
            }
        }
    }

    public override bool TryGetMember(
        GetMemberBinder binder, 
        out object? result)
    {
        if (this.items.TryGetValue(binder.Name, out result))
        {
            return true;
        }
        
        if (this.throwIfMissing)
        {
            throw new MissingKeyException(binder.Name);
        }

        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        this.items[binder.Name] = value;
        return true;
    }

    public void ShouldThrowIfKeyNotFound()
    {
        this.throwIfMissing = true;
    }

    public void Add(string key, object? value)
    {
        this.items.Add(key, value);
    }

    public bool ContainsKey(string key)
    {
        return this.items.ContainsKey(key);
    }

    public bool Remove(string key)
    {
        return this.items.Remove(key);
    }

    public bool TryGetValue(string key, out object? value)
    {
        return this.items.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<string, object?> item)
    {
        this.items.Add(item);
    }

    public void Clear()
    {
        this.items.Clear();
    }

    public bool Contains(KeyValuePair<string, object?> item)
    {
        return this.items.Contains(item);
    }

    public void CopyTo(KeyValuePair<string, object?>[] array, int arrayIndex)
    {
        this.items.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<string, object?> item)
    {
        return this.items.Remove(item);
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        return this.items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Dictionary<string, object?>(this.items).GetEnumerator();
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this.items);
    }
    
    public override string ToString()
    {
        return this.ToJson();
    }

    internal class DynamicDictionaryDebugView
    {
        private readonly IDictionary<string, object?> items;

        public DynamicDictionaryDebugView(DynamicDictionary dictionary)
        {
            this.items = dictionary.items;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<string, object?>[] Items => this.items.ToArray();
    }
}
