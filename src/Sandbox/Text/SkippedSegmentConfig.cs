namespace Sandbox.Text;

public class SkippedSegmentConfig : ISegmentConfig
{
    private readonly int length;

    public SkippedSegmentConfig(int length)
    {
        this.length = length;
    }

    public string GetPattern() => $"(.{{{this.length}}})";

    public bool TryGetName(out string name)
    {
        name = string.Empty;
        return false;
    }
}