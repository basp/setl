namespace Sandbox;

internal interface IRecord
{
    Line Line { get; }
    
    void Accept(IRecordVisitor visitor);
}