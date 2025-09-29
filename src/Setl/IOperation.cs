using Setl.Operations;

namespace Setl;

public interface IOperation : IDisposable
{
    string Name { get; }
    
    /// <summary>
    /// Occurs when a row has finished processing.
    /// </summary>
    event Action<IOperation, Row> RowProcessed;
    
    /// <summary>
    /// Occurs when the operation has finished processing.
    /// </summary>
    event Action<IOperation> FinishedProcessing;

    /// <summary>
    /// Provides statistics about the operation.
    /// </summary>
    OperationStatistics Statistics { get; }
    
    /// <summary>
    /// Prepares a operation for execution.
    /// </summary>
    /// <param name="pipelineExecutor">
    /// The <see cref="IPipelineExecutor"/> to use.
    /// </param>
    void Prepare(IPipelineExecutor pipelineExecutor);
    
    /// <summary>
    /// Executes the operation on the specified rows.
    /// </summary>
    /// <param name="rows">The input rows.</param>
    /// <returns>The output rows.</returns>
    IEnumerable<Row> Execute(IEnumerable<Row> rows);

    IEnumerable<Exception> GetErrors();

    /// <summary>
    /// Raises the <c>RowProcessed</c> event for the specified row.
    /// </summary>
    /// <param name="row">The row that just finished processing.</param>
    internal void RaiseRowProcessed(Row row);
    
    /// <summary>
    /// Raises the <c>FinishedProcessing</c> event.
    /// </summary>
    internal void RaiseFinishedProcessing();
}