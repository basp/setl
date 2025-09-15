namespace Setl.Tests;

public class DynamicDictionaryTests
{
    [Fact]
    public void DynamicObjectDictionary()
    {
        dynamic row = new Row
        {
            ["foo"] = "quux",
            ["bar"] = 123,
            ["yoy"] = 45.123,
        };

        row.foo = "yay";
        row.yoy = "quux";

        Assert.Equal("yay", row["foo"]);
        Assert.Equal("yay", row.foo);

        Assert.Equal(123, row["bar"]);
        Assert.Equal(123, row.bar);

        Assert.Equal("quux", row["yoy"]);
        Assert.Equal("quux", row.yoy);
    }

    [Fact]
    public void CaseInsensitiveKeysByDefault()
    {
        // All row keys are capitalized.
        dynamic row = new Row
        {
            ["Foo"] = "quux",
            ["Bar"] = 123,
            ["Yoy"] = 45.123,       
        };

        // Dynamic access using lowercase.
        row.foo = "yay";
        row.yoy = "zoz";
        
        // Asserts using different capitalization.
        Assert.Equal("yay", row.Foo);
        Assert.Equal("yay", row.foo);
        
        Assert.Equal("zoz", row.Yoy);
        Assert.Equal("zoz", row.yoy);
    }

    [Fact]
    public void CaseSensitiveWithCustomComparer()
    {
        dynamic row = new Row(StringComparer.InvariantCulture)
        {
            ["Foo"] = "quux",
            ["Bar"] = 123,
            ["Yoy"] = 45.123,      
        };

        row.foo = "frotz";
        
        Assert.Equal("quux", row.Foo);
        Assert.Equal("frotz", row.foo);
    }
    
    [Fact]
    public void MissingKeyDoesNotThrowException()
    {
        dynamic row = new Row
        {
            ["foo"] = "flotsam",
        };

        Assert.Null(row.bar);
        Assert.Equal("flotsam", row.foo);
    }

    [Fact]
    public void MissingKeyThrowsException()
    {
        dynamic row = new Row
        {
            ["foo"] = "flotsam",
        };
        
        row.ShouldThrowIfKeyNotFound();
        
        Assert.Throws<MissingKeyException>(() => row.bar);
    }
}