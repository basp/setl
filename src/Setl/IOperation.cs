namespace Setl;

public interface IOperation : IDisposable
{
    string Name { get; }
    
    bool UseTransaction { get; set; }

    event Action<IOperation, Row> OnRowProcessed;

    event Action<IOperation> OnFinishedProcessing;
    
    void PrepareForExecution(IPipelineExecutor pipelineExecutor);
    
    IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
    void RaiseRowProcessed(Row row);
    
    void RaiseFinishedProcessing();

    IEnumerable<Exception> GetAllErrors();
}