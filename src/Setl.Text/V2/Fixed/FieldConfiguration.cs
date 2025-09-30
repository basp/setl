using Setl.Text.V2.FieldConverters;
using Setl.Text.V2.FieldValidators;

namespace Setl.Text.V2.Fixed;

public class FieldConfiguration
{
    public string Name { get; init; } = string.Empty;
    
    public int Length { get; init; }

    public IFieldConverter Converter { get; set; } =
        new NopFieldConverter();
    
    public IFieldValidator Validator { get; set; } =
        new NopFieldValidator();
    
    public bool Skip { get; set; }
}