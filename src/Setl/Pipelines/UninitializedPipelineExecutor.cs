using Setl.Pipelines;

namespace Setl;

internal class UninitializedPipelineExecutor : IPipelineExecutor
{
    public void Execute(
        string name, 
        ICollection<IOperation> pipeline, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
        throw CreatePipelineExecutorException();

    public IEnumerable<Row> ToPipeline(
        IEnumerable<IOperation> operations, 
        IEnumerable<Row> rows, 
        Func<IEnumerable<Row>, IEnumerable<Row>> translate) =>
        throw CreatePipelineExecutorException();

    private static Exception CreatePipelineExecutorException()
    {
        // const string msg = "The pipeline executor is not correctly initialized.";
        const string msg = """
                           It seems like no actual pipeline executor has been provided.
                           In most cases, the pipeline executor is passed to the constructor of the process.
                           The client code is responsible for providing the pipeline executor.
                           Usually, it should not be possible to instantiate a process without a pipeline executor.
                           Please make sure your process is passing the pipeline executor to the base class.
                           """;
        return new InvalidOperationException(msg);
    }
}