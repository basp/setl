namespace Setl.Operations;

[Flags]
public enum JoinType
{
    Inner = 0,
    Left = 1,
    Right = 2,
    Full = JoinType.Left | JoinType.Right,
}