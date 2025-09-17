namespace Setl;

internal class ThrowingPipelineExecutor : IPipelineExecutor
{
    public void Execute(
        string name, 
        ICollection<IOperation> pipeline, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
        throw CreatePipelineExecutorException();

    public IEnumerable<Row> ToEnumerable(
        IEnumerable<IOperation> pipeline, 
        IEnumerable<Row> rows, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
        throw CreatePipelineExecutorException();

    private static Exception CreatePipelineExecutorException()
    {
        const string msg = 
            "The pipeline executor is not correctly initialized.";
        return new InvalidOperationException(msg);
    }
}