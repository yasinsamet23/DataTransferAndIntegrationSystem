namespace DataTransferAndIntegrationSystem.Application.DTOs;

public class TransferResultDto
{
    public int TotalRecords { get; set; }

    public int SuccessfulRecords { get; set; }

    public int FailedRecords { get; set; }

    public string Message { get; set; } = string.Empty;
}