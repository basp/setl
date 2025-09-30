using Setl.Text.FieldConverters;
using Setl.Text.FieldValidators;
using Setl.Text.Fixed;

namespace Setl.Cmd.Examples;

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
                .Length(5)
                .SetConverter(new Int32Converter()))
            .Field("bar", cfg => cfg
                .Length(5)
                .SetConverter(faultingConverter))
            .Field("zoz", cfg => cfg
                .Length(5)
                .AddValidator(faultingValidator)
                .SetConverter(trimConverter))
            .Field("qux", 3)
            .Field("_1", 2, skip: true)
            .Field("date", cfg => cfg
                .Length(8)
                .AddValidator(new RegexValidator(@"^[0-9]{8}$"))
                .SetConverter(new DateTimeConverter("yyyyMMdd")))
            .Build();
        
        const string text = "123  ABCD 56789XYZ__19800714";
        var row = deserializer.Deserialize(text);
        Console.WriteLine(row.ToJson());
    }
}