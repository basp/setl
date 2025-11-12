namespace Sandbox.Entities;

public record ProcessingErrorData
{
    public ProcessingErrorData(int errorCode, string organizationCode, string errorMessage)
    {
        this.ErrorCode = errorCode;
        this.OrganizationCode = organizationCode;
        this.ErrorMessage = errorMessage;
    }

    public int ErrorCode { get; set; }
    
    public string OrganizationCode { get; set; }
    
    public string ErrorMessage { get; set; } = string.Empty;
}