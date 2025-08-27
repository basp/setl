namespace Setl.Tests;

public class ObjectArrayKeysTests
{
    [Fact]
    public void TestEquality()
    {
        var first = new object[]
        {
            123,
            "foo",
            45.13,
        };

        var second = new object[]
        {
            123,
            "foo",
            45.13,
        };
        
        var third = new object[]
        {
            456,
            "foo",
            45.13,
        };
        
        var keys1 = new ObjectArrayKeys(first);
        var keys2 = new ObjectArrayKeys(second);
        var keys3 = new ObjectArrayKeys(third);

        Assert.Equal(keys1, keys2);
        Assert.Equal(keys1.GetHashCode(), keys2.GetHashCode());

        Assert.NotEqual(keys2, keys3);
        Assert.NotEqual(keys2.GetHashCode(), keys3.GetHashCode());
    }
}
