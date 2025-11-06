using Sandbox.Support;
using Sandbox.Text;

namespace Sandbox;

public class MappingDataEvaluator : IDataEvaluator
{
    private readonly Dictionary<string, Func<string, object?>> mappings;

    public MappingDataEvaluator(
        Dictionary<string, Func<string, object?>> mappings)
    {
        this.mappings = mappings;   
    }
    
    public Row Evaluate(Dictionary<string, string> data)
    {
        var row = new Row();
        foreach (var field in data)
        {
            if (this.mappings.TryGetValue(
                    field.Key, 
                    out var mapping))
            {
                row[field.Key] = mapping(field.Value);
                continue;
            }
            
            row[field.Key] = field.Value;
        }

        return row;
    }

    public bool TryEvaluate(Dictionary<string, string> data, out Row result)
    {
        result = new Row();

        try
        {
            this.Evaluate(data);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public T Evaluate<T>(Dictionary<string, string> data)
    {
        var row = this.Evaluate(data);
        return row.ToObject<T>();
    }

    public bool TryEvaluate<T>(Dictionary<string, string> data, out T? result)
    {
        result = default;

        if (!this.TryEvaluate(data, out var row))
        {
            return false;
        }
        
        result = row.ToObject<T>();
        return true;
    }
}