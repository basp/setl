namespace Setl.Text.V1;

public interface ITextDeserializer
{
    Row Deserialize(string text);

    T Deserialize<T>(string text) where T : new();
}