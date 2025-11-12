using Sandbox.Entities;

namespace Sandbox;

internal interface ILineProcessor
{
    ProcessingReportSummary Process(IEnumerable<Line> lines);
}