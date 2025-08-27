namespace Setl;

public interface IOperation
{
    string Name { get; }
    
    bool UseTransaction { get; set; }

    event Action<IOperation, Row> OnRowProcessed;

    event Action<IOperation> OnFinishedProcessing;
    
    void PrepareForExecution();
    
    IEnumerable<Row> Execute();
    
    void RaiseFinishedProcessing();

    IEnumerable<Exception> GetAllErrors();
}