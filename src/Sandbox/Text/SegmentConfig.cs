namespace Sandbox.Text;

public abstract class SegmentConfig : ISegmentConfig
{
    public abstract string GetPattern();

    public virtual bool TryGetName(out string name)
    {
        name = string.Empty;
        return false;
    }
}