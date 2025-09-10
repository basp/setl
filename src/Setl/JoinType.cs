namespace Setl;

[Flags]
public enum JoinType
{
    Inner = 0,
    Left = 1,
    Right = 2,
    Full = Left | Right,
}