namespace Setl;

// NOTE: we can probably re-use an inner SequentialTextSerializerBuilder.
public class ExplicitTextSerializerBuilder : ITextSerializerBuilder
{
    public void Field(string name, int offset, int length)
    {
        throw new NotImplementedException();   
    }
    
    public ITextSerializer Build()
    {
        throw new NotImplementedException();
    }
}