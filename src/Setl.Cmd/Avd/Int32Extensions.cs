namespace Setl.Cmd.Avd;

public static class Int32Extensions
{
    public static bool Validate11Proef(this int value)
    {
        var divisor = 1_000_000_000;
        var total = 0;
        var result = value;
        for (var i = 9; i > 1; i--)
        {
            total += i * Math.DivRem(result, divisor /= 10, out result);
        }

        Math.DivRem(total, 11, out var rest);
        return (result == rest);
    }
}