namespace Setl.Tests;

using System.Text.Json;

public class RowTests
{
    private static readonly JsonSerializerOptions RowSerializerOptions = 
        new()
        {
            PropertyNameCaseInsensitive = true,
        };
    
    private record TestItem
    {
        public string? Foo { get; init; }
        public int Bar { get; init; }
        public double Yoy { get; init; }    
    }
    
    [Fact]
    public void CreateRowFromObject()
    {
        var obj = new TestItem
        {
            Foo = "quux",
            Bar = 123,
            Yoy = 45.123,       
        };
        
        var row = Row.FromObject(obj);

        Assert.Equal("quux", row["foo"]);
        Assert.Equal(123, row["bar"]);
        Assert.Equal(45.123, row["yoy"]);
    }

    [Fact]
    public void ConvertRowToObject()
    {
        var row = new Row
        {
            ["foo"] = "quux",
            ["bar"] = 123,
            ["yoy"] = 45.123
        };

        var obj = row.ToObject<TestItem>();
        
        Assert.Equal("quux", obj.Foo);
        Assert.Equal(123, obj.Bar);
        Assert.Equal(45.123, obj.Yoy);
    }

    [Fact]
    public void SerializeRowToJson()
    {
        var row = new Row
        {
            ["foo"] = "quux",
            ["bar"] = 123,
            ["yoy"] = 45.123
        };
        
        var json = row.ToJson();
        var obj = JsonSerializer.Deserialize<TestItem>(
            json, 
            RowSerializerOptions)!;
        
        Assert.Equal("quux", obj.Foo);
        Assert.Equal(123, obj.Bar);
        Assert.Equal(45.123, obj.Yoy);   
    }
}
