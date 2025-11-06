using Sandbox.Records;
using Sandbox.Support;

namespace Sandbox;

internal class RowConverters
{
    public static Berichtrecord ToBerichtrecord(Row row) =>
        row.ToObject<Berichtrecord>();
    
    public static Gemeenterecord ToGemeenterecord(Row row) =>
        row.ToObject<Gemeenterecord>();
    
    public static Detailrecord ToDetailrecord(Row row) =>
        row.ToObject<Detailrecord>();
    
    public static Tellingenrecord ToTellingenrecord(Row row) =>
        row.ToObject<Tellingenrecord>();
}