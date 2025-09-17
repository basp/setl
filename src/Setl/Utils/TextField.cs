namespace Setl.Utils;

public class TextField
{
    private static readonly Func<string, object?> DefaultConverter = 
        x => x.Trim();

    public TextField()
        : this(string.Empty, 0)
    {
    }
    
    public TextField(string name, int length)
        : this(name, length, TextField.DefaultConverter)
    {
    }

    public TextField(
        string name,
        int length,
        Func<string, object?> convert)
    {
        this.Name = name;
        this.Length = length;
        this.Convert = convert;
    }

    public string Name { get; set; }
    
    public int Length { get; set; }
    
    public Func<string, object?> Convert { get; set; }
    
    public bool Skip { get; set; }
}