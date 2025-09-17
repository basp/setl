namespace Setl;

public interface IMissingKeyBehavior
{
    public void Handle(
        string key, 
        DynamicDictionary dictionary);
}