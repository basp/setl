namespace Setl.Utils;

public interface ITextSerializer
{
    Row Deserialize(string text);

    T Deserialize<T>(string text) where T : new();
}