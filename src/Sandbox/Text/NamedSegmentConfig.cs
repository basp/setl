namespace Sandbox.Text;

internal class NamedSegmentConfig : ISegmentConfig
{
    private readonly string name;
    private readonly int length;

    public NamedSegmentConfig(string name, int length)
    {
        this.name = name;
        this.length = length;
    }

    public string GetPattern() => $"(?<{this.name}>.{{{this.length}}})";

    public bool TryGetName(out string value)
    {
        value = this.name;
        return true;
    }
}