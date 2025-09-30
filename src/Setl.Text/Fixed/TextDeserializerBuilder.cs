using System.Text;
using System.Text.RegularExpressions;
using Setl.Text.FieldConverters;

namespace Setl.Text.Fixed;

public class TextDeserializerBuilder
{
    private readonly List<FieldConfiguration> fields = []; 
    
    private bool autoTrim = true;
    
    public TextDeserializerBuilder Field(
        string name, 
        int length, 
        bool skip = false) =>
            this.Field(name, cfg =>
            {
                cfg.Length(length);
                cfg.Skip(skip);       
            });
    
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

    /// <summary>
    /// Setting <c>AutoTrim</c> to <c>true</c> (default) will trim leading and
    /// trailing whitespace from the (validated) field values <b>before</b>
    /// sending them to the convertor.
    /// </summary>
    /// <param name="value">
    /// The new value for the <c>AutoTrim</c> setting.
    /// </param>
    /// <returns>
    /// The <see cref="TextDeserializerBuilder"/>
    /// </returns>
    public TextDeserializerBuilder AutoTrim(bool value)
    {
        this.autoTrim = value;
        return this;
    }
    
    public ITextDeserializer Build()
    {
        var regex = this.BuildRegex();
        return new TextDeserializer(regex, this.fields, this.autoTrim);
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
        private bool skip;
        private readonly List<IFieldValidator> validators = [];
        private IFieldConverter converter = new NopFieldConverter();

        public FieldConfigurationBuilder(string name)
        {
            this.name = name;
        }
        
        public IFieldConfigurationBuilder Length(int value)
        {
            this.length = value;
            return this;
        }
        
        public IFieldConfigurationBuilder Skip(bool value)
        {
            this.skip = value;
            return this;
        }

        public IFieldConfigurationBuilder SetConverter(IFieldConverter value)
        {
            this.converter = value;
            return this;
        }

        public IFieldConfigurationBuilder AddValidator(IFieldValidator value)
        {
            this.validators.Add(value);
            return this;
        }

        public FieldConfiguration Build()
        {
            return new FieldConfiguration()
            {
                Name = this.name,
                Length = this.length,
                Skip = this.skip,
                Converter = this.converter,
                Validators = this.validators,
            };
        }
    }
}