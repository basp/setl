#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal class Detailrecord : AowAioRecord
{
    public string Gemeentecode { get; set;  }
    
    public string SofinummerHp { get; set; }

    public string AchternaamHp { get; set; }

    public string VoorlettersHp { get; set; }

    public string VoorvoegselHp { get; set; }

    public string EersteVoornaamHp { get; set; }
    
    public DateOnly GeboortedatumHp { get; set; }
    
    public string RekeningnummerHp { get; set; }
    
    public int WWBBedragHp { get; set; }
    
    public string PostcodeNumeriek { get; set; }
    
    public string PostcodeLetters { get; set; }
    
    public int Huisnummer { get; set; }
    
    public string HuisnummerToevoeging { get; set; }
    
    public string Straatnaam { get; set; }
    
    public string Plaatsnaam { get; set; }
    
    public string SofinummerP { get; set; }
    
    public string AchternaamP { get; set; }
    
    public string VoorlettersP { get; set; }
    
    public string VoorvoegselP { get; set; }
    
    public string EersteVoornaamP { get; set; }
    
    public DateOnly? GeboortedatumP { get; set; }
    
    public string RekeningnummerP { get; set; }
    
    public int? WWBBedragP { get; set; }
    
    public DateOnly IngangsdatumRecht { get; set; }
    
    public DateOnly? EinddatumRecht { get; set; }

    public string WWBNorm { get; set; }

    public override void Accept(IRecordVisitor visitor)
    {
        visitor.VisitDetail(this);
    }
}
