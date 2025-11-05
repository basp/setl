namespace Sandbox.Text;

public class RestSegmentConfig : ISegmentConfig
{
    private readonly string name;

    public RestSegmentConfig(string name)
    {
        this.name = name;
    }

    public string GetPattern() => $"(?<{this.name}>.*)$";

    public bool TryGetName(out string value)
    {
        value = this.name;
        return true;
    }
}