namespace Sandbox;

public record ProcessingReportSummary
{
    public string GeneralErrorMessage { get; set; } = string.Empty;

    public bool IsValid { get; init; }
    
    public int TotalNumberOfRecords { get; init; }

    public int NumberOfProcessedRecords { get; init; }
    
    public int NumberOfFailedRecords { get; init; }
}