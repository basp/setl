namespace Sandbox;

internal interface IRecord
{
    void Accept(IRecordVisitor visitor);
}