namespace Sandbox;

internal interface ILineProcessor
{
    ProcessingReportSummary Process(IEnumerable<Line> lines);
}