namespace Setl;

public interface IPipelineExecutor
{
    void Execute(
        string name,
        ICollection<IOperation> pipeline,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);

    IEnumerable<Row> ToEnumerable(
        IEnumerable<IOperation> pipeline,
        IEnumerable<Row> rows,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);
}