#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal class Detailrecord : AowAioRecord
{
    public string Gemeentecode { get; set;  }
    
    public string SofinummerHp { get; set; }

    public string AchternaamHp { get; set; }

    public string VoorlettersHp { get; set; }

    public string VoorvoegselHp { get; set; }
}
