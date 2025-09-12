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
}