using System.Runtime.InteropServices.ComTypes;
using Microsoft.Extensions.Logging;
using Setl.Operations;

namespace Setl.Cmd.Avd.Operations;

public class PadFieldsOperation : AbstractOperation
{
    public Action<int, string, string, string, string> OnPaddingAdded { get; init; } =
        (_, _, _, _, _) => { };

    public PadFieldsOperation(ILogger logger) : base(logger)
    {
    }

    public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
    {
        const int BSN_LENGTH = 9;
        const int GEMEENTECODE_LENGTH = 4;

        var bsn = string.Empty;
        var gemeentecode = string.Empty;
        
        foreach (var row in rows)
        {
            var originalBsn = row.GetString(FieldNames.Bsn, string.Empty);
            if (!string.IsNullOrEmpty(originalBsn) && originalBsn.Length < BSN_LENGTH)
            {
                bsn = originalBsn.PadLeft(BSN_LENGTH, '0');
                this.OnPaddingAdded(
                    row.GetInt32(FieldNames.Index), 
                    FieldNames.Bsn,
                    originalBsn,
                    bsn,
                    row.GetString(FieldNames.Source));
            }
            
            var originalGemeentecode = row.GetString(FieldNames.Gemeentecode, string.Empty);
            if (!string.IsNullOrEmpty(originalGemeentecode) && originalGemeentecode.Length < GEMEENTECODE_LENGTH)
            {
                gemeentecode = originalGemeentecode.PadLeft(GEMEENTECODE_LENGTH, '0');
                this.OnPaddingAdded(
                    row.GetInt32(FieldNames.Index), 
                    FieldNames.Gemeentecode,
                    originalGemeentecode,
                    gemeentecode,
                    row.GetString(FieldNames.Source));
            }

            yield return new Row(row)
            {
                [FieldNames.Bsn] = bsn,
                [FieldNames.Gemeentecode] = gemeentecode,
            };
        }
    }
}