namespace Setl;

public interface IOperation : IDisposable
{
    string Name { get; }

    event Action<IOperation> StartedProcessing;
    
    event Action<IOperation, Row> RowProcessed;
    
    event Action<IOperation> FinishedProcessing;

    void Prepare(IPipelineExecutor pipelineExecutor);
    
    IEnumerable<Row> Execute(IEnumerable<Row> rows);
    
    void RaiseRowProcessed(Row row);
    
    void RaiseFinishedProcessing();
}