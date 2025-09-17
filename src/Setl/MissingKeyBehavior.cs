namespace Setl;

public class MissingKeyBehavior : IMissingKeyBehavior
{
    public static readonly IMissingKeyBehavior Ignore =
        new MissingKeyBehavior((_, _) => { });

    public static readonly IMissingKeyBehavior Throw =
        new MissingKeyBehavior((key, _) =>
        {
            var msg = $"Key '{key}' not found.";
            throw new KeyNotFoundException(msg);
        });
    
    private readonly Action<string, DynamicDictionary> handler;

    private MissingKeyBehavior(Action<string, DynamicDictionary> handler)
    {
        this.handler = handler;
    }
    
    public void Handle(string key, DynamicDictionary row) => 
        this.handler(key, row);
}