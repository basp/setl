using System.Text;
using System.Text.RegularExpressions;

namespace Setl.Text.V2.Fixed;

public class TextDeserializerBuilder
{
    private readonly List<FieldConfiguration> fields = []; 
    
    public TextDeserializerBuilder Field(string name, int length)
    {
        var builder = new FieldConfigurationBuilder(name);
        builder.SetLength(length);
        var config = builder.Build();
        this.fields.Add(config);
        return this;
    }
    
    public TextDeserializerBuilder Field(
        string name,
        Action<IFieldConfigurationBuilder> configure)
    {
        var builder = new FieldConfigurationBuilder(name);
        configure(builder);
        var config = builder.Build();
        this.fields.Add(config);
        return this;
    }
    
    public TextDeserializer Build()
    {
        var regex = this.BuildRegex();
        return new TextDeserializer(regex, this.fields);
    }

    private Regex BuildRegex()
    {
        var patternBuilder = new StringBuilder();
        foreach (var field in this.fields)
        {
            var fieldPattern = field.Skip
                // If we are skipping the field, we don't need to capture it
                // in a named group. Just use an unnamed group.
                ? $"(.{{{field.Length}}})"
                // Otherwise, capture it in a named group.
                : $"(?<{field.Name}>.{{{field.Length}}})";
            
            patternBuilder.Append(fieldPattern);
        }
        
        var pattern = patternBuilder.ToString();
        return new Regex(pattern);        
    }
    
    private class FieldConfigurationBuilder : IFieldConfigurationBuilder
    {
        private readonly string name;
        private int length;
        private IFieldValidator validator = new NopFieldValidator();
        private IFieldConverter converter = new NopFieldConverter();

        public FieldConfigurationBuilder(string name)
        {
            this.name = name;
        }
        
        public IFieldConfigurationBuilder SetLength(int value)
        {
            this.length = value;
            return this;
        }

        public IFieldConfigurationBuilder SetConverter(IFieldConverter value)
        {
            this.converter = value;
            return this;
        }

        public IFieldConfigurationBuilder SetValidator(IFieldValidator value)
        {
            this.validator = value;
            return this;
        }

        public FieldConfiguration Build()
        {
            return new FieldConfiguration()
            {
                Name = this.name,
                Length = this.length,
                Converter = this.converter,
                Validator = this.validator,
            };
        }
    }
}