namespace Setl.Tests;

public class KeyTests
{
    [Fact]
    public void SameValuesSameHash()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "foo", 3.14);
        
        Assert.Equal(k1.GetHashCode(), k2.GetHashCode());
    }

    [Fact]
    public void SameValuesEqualRows()
    {
        var k1 = new Key(1, "foo", 3.14);
        var k2 = new Key(1, "foo", 3.14);

        Assert.Equal(k1, k2);
    }
}