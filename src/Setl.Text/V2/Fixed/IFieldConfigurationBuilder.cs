namespace Setl.Text.V2.Fixed;

public interface IFieldConfigurationBuilder
{
    IFieldConfigurationBuilder Length(int length);
    
    IFieldConfigurationBuilder Converter(IFieldConverter converter);
    
    IFieldConfigurationBuilder Validator(IFieldValidator validator);
    
    IFieldConfigurationBuilder Skip(bool skip);
    
    FieldConfiguration Build();
}