namespace Setl.Text.V2.Fixed;

public interface IFieldConfigurationBuilder
{
    IFieldConfigurationBuilder Length(int length);
    
    IFieldConfigurationBuilder SetConverter(IFieldConverter converter);
    
    IFieldConfigurationBuilder AddValidator(IFieldValidator validator);
    
    IFieldConfigurationBuilder Skip(bool skip);
    
    internal FieldConfiguration Build();
}