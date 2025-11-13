#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
namespace Sandbox.Records;

internal abstract record AowAioRecord : IRecord
{
    public Line Line { get; init; }

    public abstract void Accept(IRecordVisitor visitor);
}
