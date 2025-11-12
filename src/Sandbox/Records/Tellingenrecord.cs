namespace Sandbox.Records;

internal class Tellingenrecord : AowAioRecord
{
    public string Gemeentecode { get; set; } = string.Empty;

    public int TotaalAantalGerechtigden { get; set; }

    public int TotaalAantalHuishoudens { get; set; }

    public int TotaalWwbBedrag { get; set; }
    
    public override void Accept(IRecordVisitor visitor)
    {
        visitor.VisitTellingen(this);
    }
}
