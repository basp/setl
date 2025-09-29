namespace Setl;

/// <inheritdoc/>
public class MissingKeyBehavior : IMissingKeyBehavior
{
    /// <summary>
    /// Ignore the missing key and return <c>null</c>, this is considered
    /// normal behavior.
    /// </summary>
    public static readonly IMissingKeyBehavior Ignore =
        new MissingKeyBehavior((_, _) => null);

    /// <summary>
    /// Throw an exception, this is considered exceptional behavior.
    /// </summary>
    public static readonly IMissingKeyBehavior Throw =
        new MissingKeyBehavior((key, _) =>
        {
            var msg = $"The given key '{key}' was not present in the dictionary.";
            throw new KeyNotFoundException(msg);
        });
    
    private readonly Func<string, DynamicDictionary, object?> handler;

    private MissingKeyBehavior(
        Func<string, DynamicDictionary, object?> handler)
    {
        this.handler = handler;
    }
    
    /// <inheritdoc/>
    public object? Handle(string key, DynamicDictionary dictionary) => 
        this.handler(key, dictionary);
}