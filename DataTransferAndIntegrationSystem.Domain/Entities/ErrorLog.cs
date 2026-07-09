namespace DataTransferAndIntegrationSystem.Domain.Entities;

public class ErrorLog
{
    public Guid Id { get; set; }

    public Guid RecordId { get; set; }

    public string ErrorField { get; set; } = string.Empty;

    public string ErrorMessage { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
}