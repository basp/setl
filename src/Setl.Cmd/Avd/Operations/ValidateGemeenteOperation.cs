using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Avd.Operations;

public class ValidateGemeenteOperation : AbstractOperation
{
    public Action<int, string, string> OnInvalidGemeentecode { get; init; } =
        (_, _, _) => { };
    
    public ValidateGemeenteOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            var gemeentecode = row.GetString(FieldNames.Gemeentecode);
            if (!int.TryParse(gemeentecode, out var _))
            {
                this.OnInvalidGemeentecode(
                    row.GetInt32(FieldNames.Index), 
                    row.GetString(FieldNames.Gemeentecode), 
                    row.GetString(FieldNames.Source));
            }
            
            yield return row;       
        }
    }
}