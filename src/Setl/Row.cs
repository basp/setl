namespace Setl;

using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Reflection;

public class Row : DynamicDictionary, IEquatable<Row>
{
    private static readonly Dictionary<Type, List<PropertyInfo>> propertyCache = new();

    private static readonly Dictionary<Type, List<FieldInfo>> fieldCache = new();

    public Row(StringComparer comparer)
        : base(new Dictionary<string, object?>(), comparer)
    {
    }

    public Row()
        : base(new Dictionary<string, object?>())
    {
    }

    public Row(IDictionary<string, object?> items, StringComparer comparer)
        : base(items, comparer)
    {
    }

    public void Copy(IDictionary<string, object?> source)
    {
        this.items = new Dictionary<string, object?>(source, this.Comparer);
    }

    public IEnumerable<string> Columns
    {
        get
        {
            foreach (var column in new List<string>(this.items.Keys))
            {
                yield return column;
            }
        }
    }

    public Row Clone()
    {
        var row = new Row(this, this.Comparer);
        return row;
    }

    public bool Equals(Row? other)
    {
        if (other == null)
        {
            return false;
        }

        if (!this.Comparer.Equals(other.Comparer))
        {
            return false;
        }

        if (!this.Columns.SequenceEqual(other.Columns, this.Comparer))
        {
            return false;
        }

        foreach (var key in this.items.Keys)
        {
            var item = this.items[key];
            var otherItem = other.items[key];

            if (item == null || otherItem == null)
            {
                return item == null && otherItem == null;
            }

            var compare = CreateComparer(
                item.GetType(),
                otherItem.GetType());

            if (!compare(item, otherItem))
            {
                return false;
            }
        }

        return true;
    }

    public CompositeKey CreateKey()
    {
        return this.CreateKey(this.Columns.ToArray());
    }

    public CompositeKey CreateKey(params string[] columns)
    {
        var array = new object?[columns.Length];
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            array[i] = this.items[column];
        }

        return new CompositeKey(array);
    }

    public static Row FromObject(object? obj)
    {
        if (obj == null)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
        }

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

    public T ToObject<T>()
    {
        return (T)this.ToObject(typeof(T));
    }

    public bool TryGetInt(string key, out int value)
    {
        if (this.items.TryGetValue(key, out var obj))
        {
        }

        throw new NotImplementedException();
    }

    public bool TryGetString(string key, out string? value)
    {
        value = null;
        
        if (this.items.TryGetValue(key, out var obj))
        {
            value = obj as string;
        }
        
        return value != null;
    }

    private static List<PropertyInfo> GetProperties(object obj)
    {
        var type = obj.GetType();
        if (Row.propertyCache.TryGetValue(type, out var properties))
        {
            return properties;
        }

        var bindingFlags =
            BindingFlags.Public |
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

        var bindingFlags =
            BindingFlags.Public |
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