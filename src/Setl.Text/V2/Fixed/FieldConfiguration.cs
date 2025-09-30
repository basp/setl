using Setl.Text.V2.FieldConverters;
using Setl.Text.V2.FieldValidators;

namespace Setl.Text.V2.Fixed;

public class FieldConfiguration
{
    public string Name { get; init; } = string.Empty;
    
    public int Length { get; init; }

    public IFieldConverter Converter { get; init; } =
        new NopFieldConverter();

    public List<IFieldValidator> Validators { get; set;  } = [];
    
    public bool Skip { get; init; }
}