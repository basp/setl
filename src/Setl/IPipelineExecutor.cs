namespace Setl;

public interface IPipelineExecutor
{
    void Execute(
        string name,
        ICollection<IOperation> pipeline,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);

    IEnumerable<Row> ToPipeline(
        IEnumerable<IOperation> operations,
        IEnumerable<Row> rows,
        Func<IEnumerable<Row>, IEnumerable<Row>> translate);
}