#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal class Gemeenterecord : AowAioRecord
{
    public int Gemeentecode { get; set; }

    public int Verwerkingsjaar { get; set; }

    public int Verwerkingsmaand { get; set; }
}
