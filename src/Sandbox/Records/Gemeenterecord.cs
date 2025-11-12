#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal class Gemeenterecord : AowAioRecord
{
    public string Gemeentecode { get; set; } = string.Empty;

    public int Verwerkingsjaar { get; set; }

    public int Verwerkingsmaand { get; set; }
    
    public override void Accept(IRecordVisitor visitor)
    {
        visitor.VisitGemeente(this);
    }
}
