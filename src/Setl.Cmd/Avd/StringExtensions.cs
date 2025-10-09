namespace Setl.Cmd.Avd;

public static class StringExtensions
{
    public static bool Validate11Proef(this string self)
    {
        if (int.TryParse(self, out var value))
        {
            return value.Validate11Proef();
        }

        return false;
    }
}