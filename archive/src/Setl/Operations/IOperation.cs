namespace Setl.Operations;

public interface IOperation : IDisposable
{
    string Name { get; }
    
    bool UseTransaction { get; set; }

    event Action<IOperation, Row> RowProcessed;

    event Action<IOperation> FinishedProcessing;
    
    void PrepareForExecution(IPipelineExecutor pipelineExecutor);
    
    IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
    void RaiseRowProcessed(Row row);
    
    void RaiseFinishedProcessing();

    IEnumerable<Exception> GetAllErrors();
}