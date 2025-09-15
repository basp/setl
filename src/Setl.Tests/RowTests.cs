// ReSharper disable UnusedAutoPropertyAccessor.Local
namespace Setl.Tests;

public class RowTests
{
    [Fact]
    public void CreateEmptyRow()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var row = new Row();
        
        Assert.Empty(row);
    }

    [Fact]
    public void CreateRowWithValues()
    {
        var values = new Dictionary<string, object?>
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var row = new Row(values);
        
        Assert.Equal(3, row.Count);
        Assert.Equal(1, row["a"]);
        Assert.Equal(2, row["b"]);
        Assert.Equal(3, row["c"]);
    }
    
    [Fact]
    public void SameRowsAreEqual()
    {
        var row1 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };

        var row2 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        Assert.Equal(row1, row2);
        Assert.True(row1.Equals(row2));
    }

    [Fact]
    public void DifferentRowsNotEqual()
    {
        var row1 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var row2 = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 4,
        };
        
        Assert.NotEqual(row1, row2);
        Assert.False(row1.Equals(row2));
    }

    [Fact]
    public void CloneRowResultsInNewRow()
    {
        var a = new Row
        {
            ["a"] = 1,
            ["b"] = 2,
            ["c"] = 3,
        };
        
        var b = a.Clone();
        b["a"] = 4;
        
        Assert.False(ReferenceEquals(a, b));
        Assert.Equal(1, a["a"]);
        Assert.Equal(4, b["a"]);
    }

    [Fact]
    public void CreateRowFromObject()
    {
        var obj = new
        {
            Id = 1,
            Name = "foo",
            Value = 3.14,
        };

        var row = Row.FromObject(obj);
        Assert.Equal(1, row["Id"]);
        Assert.Equal("foo", row["Name"]);
        Assert.Equal(3.14, row["Value"]);
    }

    [Fact]
    public void ConvertRowToObject()
    {
        var row = new Row
        {
            ["Id"] = 1,
            ["Name"] = "foo",
            ["Value"] = 3.14,
        };
        
        var obj = row.ToObject<Foo>();
        Assert.Equal(1, obj.Id);
        Assert.Equal("foo", obj.Name);
        Assert.Equal(3.14, obj.Value);
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Foo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Value { get; set; }
    }
}