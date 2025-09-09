namespace Setl;

public interface IPipelineExecutor
{
    bool HasErrors { get; }
    
    void Execute(
        string name,
        ICollection<IOperation> pipeline,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);

    IEnumerable<Row> PipelineToEnumerable(
        IEnumerable<IOperation> pipeline,
        IEnumerable<Row> rows,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);
    
    IEnumerable<Exception> GetAllErrors();
}