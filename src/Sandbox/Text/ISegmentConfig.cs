namespace Sandbox.Text;

public interface ISegmentConfig
{
    string GetPattern();

    bool TryGetName(out string name);
}