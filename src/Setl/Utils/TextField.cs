namespace Setl.Utils;

public class TextField
{
    public TextField()
        : this(string.Empty, 0)
    {
    }
    
    public TextField(string name, int length)
        : this(name, length, new RawFieldConverter())
    {
    }

    public TextField(
        string name,
        int length,
        IFieldConverter converter)
    {
        this.Name = name;
        this.Length = length;
        this.Converter = converter;
    }

    public string Name { get; set; }
    
    public int Length { get; set; }
    
    public IFieldConverter Converter { get; set; }
    
    public bool Skip { get; set; }
}