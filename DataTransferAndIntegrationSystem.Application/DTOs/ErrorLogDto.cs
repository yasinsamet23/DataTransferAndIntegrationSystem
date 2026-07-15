namespace DataTransferAndIntegrationSystem.Application.DTOs;

public class ErrorLogDto
{
    public Guid RecordId { get; set; }

    public string ErrorField { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;
}