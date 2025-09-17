using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Setl;

/// <summary>
/// Represents a row of data.
/// </summary>
public class Row : DynamicDictionary, IEquatable<Row>
{
    // The caches help to avoid reflection in the GetProperties and GetFields
    // methods.
    private static readonly Dictionary<Type, List<PropertyInfo>> propertyCache = new();
    private static readonly Dictionary<Type, List<FieldInfo>> fieldCache = new();
    
    /// <summary>
    /// Create a new empty row.
    /// </summary>
    public Row()
    {
    }

    /// <summary>
    /// Create a new row from an existing dictionary.
    /// </summary>
    /// <param name="items">Dictionary.</param>
    public Row(IDictionary<string, object?> items)
        : base(items)
    {
    }

    /// <summary>
    /// Create a new row with a specific comparer.
    /// </summary>
    /// <param name="comparer">Comparer</param>
    public Row(StringComparer comparer)
        : base(comparer)
    {
    }

    /// <summary>
    /// Create a new row from an existing dictionary and a specific comparer.
    /// </summary>
    /// <param name="items">Dictionary.</param>
    /// <param name="comparer">Comparer.</param>
    public Row(IDictionary<string, object?> items, StringComparer comparer)
        : base(items, comparer)
    {
    }

    // ReSharper disable once MemberCanBePrivate.Global
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
    
    /// <summary>
    /// Crates a new row from an object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns>Row.</returns>
    public static Row FromObject(object obj)
    {
        var row = new Row();

        var properties = GetProperties(obj);
        var fields = GetFields(obj);

        foreach (var property in properties)
        {
            row[property.Name] = property.GetValue(obj);
        }

        foreach (var field in fields)
        {
            row[field.Name] = field.GetValue(obj);
        }

        return row;
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

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }
        
        if (obj is not Row other)
        {
            return false;
        }
        
        return this.Equals(other);
    }

    public override int GetHashCode() => this.CreateKey().GetHashCode();
    
    public bool Equals(Row? other)
    {
        if (other is null)
        {
            return false;
        }

        if (!this.comparer.Equals(other.comparer))
        {
            return false;
        }

        if (!this.Columns.SequenceEqual(other.Columns))
        {
            return false;
        }

        foreach (var key in this.items.Keys)
        {
            var thisItem = this.items[key];
            var otherItem = other.items[key];

            if (thisItem is null || otherItem is null)
            {
                return thisItem == null && otherItem == null;
            }
            
            var compare = CreateComparer(
                thisItem.GetType(), 
                otherItem.GetType());

            if (!compare(thisItem, otherItem))
            {
                return false;
            }
        }

        return true;
    }
    public object ToObject(Type type)
    {
        var instance = Activator.CreateInstance(type)!;

        foreach (var info in GetProperties(instance))
        {
            if (this.items.TryGetValue(info.Name, out var value) && info.CanWrite)
            {
                info.SetValue(instance, value);
            }
        }

        foreach (var info in GetFields(instance))
        {
            if (this.items.TryGetValue(info.Name, out var value))
            {
                info.SetValue(instance, value);
            }
        }

        return instance;
    }

    public T ToObject<T>() => (T)this.ToObject(typeof(T));
    
    public string ToJson(JsonSerializerOptions? options = null) =>
        JsonSerializer.Serialize(this, options);
    
    private static List<PropertyInfo> GetProperties(object obj)
    {
        var type = obj.GetType();
        if (Row.propertyCache.TryGetValue(type, out var properties))
        {
            return properties;
        }

        const BindingFlags bindingFlags = BindingFlags.Public |
                                          BindingFlags.Instance |
                                          BindingFlags.NonPublic;
        
        properties = [];
        foreach (var property in type.GetProperties(bindingFlags))
        {
            if (!property.CanRead || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            properties.Add(property);
        }

        propertyCache[type] = properties;
        return properties;
    }

    private static List<FieldInfo> GetFields(object obj)
    {
        var type = obj.GetType();
        if (fieldCache.TryGetValue(type, out var fields))
        {
            return fields;
        }

        const BindingFlags bindingFlags = BindingFlags.Public |
                                          BindingFlags.Instance |
                                          BindingFlags.NonPublic;
        
        fields = [];
        foreach (var field in type.GetFields(bindingFlags))
        {
            // Skip compiler-generated fields.
            if (Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute)))
            {
                continue;
            }

            fields.Add(field);
        }

        fieldCache[type] = fields;
        return fields;
    }
    
    private static Func<object, object, bool> CreateComparer(
        Type firstType,
        Type secondType)
    {
        if (firstType == secondType)
        {
            return Equals;
        }

        var firstParam =
            Expression.Parameter(typeof(object), "first");
        var secondParam =
            Expression.Parameter(typeof(object), "second");
        
        var expression =
            Expression.Equal(
                Expression.Convert(firstParam, firstType),
                Expression.Convert(secondParam, secondType));
        
        return Expression
            .Lambda<Func<object, object, bool>>(
                expression,
                firstParam,
                secondParam)
            .Compile();
    }
}