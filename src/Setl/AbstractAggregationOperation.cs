using Microsoft.Extensions.Logging;

namespace Setl;

public abstract class AbstractAggregationOperation : AbstractOperation
{
    protected AbstractAggregationOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        var aggregations = new Dictionary<CompositeKey, Row>();
        var groupBy = this.GetColumnsToGroupBy();
        foreach (var row in rows)
        {
            var key = row.CreateKey(groupBy);
            if (!aggregations.TryGetValue(key, out var aggregate))
            {
                aggregate = new Row();
                aggregations[key] = aggregate;
            }
            
            this.Accumulate(row, aggregate);
        }

        foreach (var row in aggregations.Values)
        {
            this.FinishAggregation(row);
            yield return row;
        }
    }
    
    protected virtual void FinishAggregation(Row aggregate)
    {
    }
    
    protected abstract void Accumulate(Row row, Row aggregate);
    
    protected virtual string[] GetColumnsToGroupBy() => [];
}