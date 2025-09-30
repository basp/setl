namespace Setl.Text.V2.Fixed;

public interface IFieldConfigurationBuilder
{
    IFieldConfigurationBuilder SetLength(int length);
    
    IFieldConfigurationBuilder SetConverter(IFieldConverter converter);
    
    IFieldConfigurationBuilder SetValidator(IFieldValidator validator);
    
    FieldConfiguration Build();
}