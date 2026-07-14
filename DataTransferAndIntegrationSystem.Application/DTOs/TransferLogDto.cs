namespace DataTransferAndIntegrationSystem.Application.DTOs;

public class TransferLogDto
{
    public Guid Id { get; set; }

    public DateTime TransferDate { get; set; }

    public int TotalRecords { get; set; }

    public int SuccessCount { get; set; }

    public string Status { get; set; } = string.Empty;
}