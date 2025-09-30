using Setl.Text.V2;
using Setl.Text.V2.FieldConverters;
using Setl.Text.V2.FieldValidators;
using Setl.Text.V2.Fixed;

namespace Setl.Cmd.Examples.V2;

public static class TextDeserializerExample
{
    public static void Run()
    {
        var trimConverter = new FieldConverter
        {
            TryConvert = value => (true, value.Trim()),
        };

        var faultingConverter = new FieldConverter
        {
            TryConvert = value => (false, value),
            FormatErrorMessage = (name, value) => 
                $"Invalid value for {name}: {value}",
        };

        var faultingValidator = new FieldValidator
        {
            Validate = value => false,
            FormatErrorMessage = (name, value) => 
                $"Invalid value for {name}: {value}",       
        };
        
        var builder = new TextDeserializerBuilder();
        var deserializer = builder
            .Field("foo", cfg => cfg
                .SetLength(5)
                .SetConverter(faultingConverter))
            .Field("bar", cfg => cfg
                .SetLength(5)
                .SetConverter(faultingConverter))
            .Field("zoz", cfg => cfg
                .SetLength(5)
                .SetValidator(faultingValidator)
                .SetConverter(trimConverter))
            .Field("qux", 3)
            .Build();
        
        const string text = "123  ABCD 56789XYZ";
        var row = deserializer.Deserialize(text);
        Console.WriteLine(row.ToJson());
    }
}