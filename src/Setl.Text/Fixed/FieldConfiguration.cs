using Setl.Text.FieldConverters;

namespace Setl.Text.Fixed;

public class FieldConfiguration
{
    public string Name { get; init; } = string.Empty;
    
    public int Length { get; init; }

    public IFieldConverter Converter { get; init; } =
        new NopFieldConverter();

    public List<IFieldValidator> Validators { get; set;  } = [];
    
    public bool Skip { get; init; }
}