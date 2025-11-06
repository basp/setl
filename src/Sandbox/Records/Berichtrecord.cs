#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal class Berichtrecord : AowAioRecord
{
    public string Berichttype { get; set; }

    public string FunctieVersie { get; set; }

    public string NaamBericht { get; set; }

    public string CodeSectorLeverancier { get; set; }

    public string CodeSectorAanvrager { get; set; }

    public DateOnly DatumAanmaakBericht { get; set; }

    public string ReferentieLevering { get; set; }
}
