namespace Sandbox.Text;

internal interface IFixedWidthParser
{
    Dictionary<string, string> Parse(string text);
    
    bool TryParse(string text, out Dictionary<string, string> result);
}