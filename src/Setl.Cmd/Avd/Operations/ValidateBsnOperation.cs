using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Avd.Operations;

public class ValidateBsnOperation : AbstractOperation
{
    public Action<int, string, string> OnInvalidBsn { get; init; } =
        (_, _, _) => { };

    public ValidateBsnOperation(ILogger logger) 
        : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        foreach (var row in rows)
        {
            var bsn = row.GetString(FieldNames.Bsn);
            if (!bsn.Validate11Proef())
            {
                this.OnInvalidBsn(
                    row.GetInt32(FieldNames.Index), 
                    row.GetString(FieldNames.Bsn), 
                    row.GetString(FieldNames.Source));

                continue;
            }

            yield return row;
        }
    }
}