namespace DataTransferAndIntegrationSystem.Application.DTOs;

public class AnomalyResultDto
{
    public List<AnomalyErrorDto> Errors { get; set; } = [];

    public bool IsValid => Errors.Count == 0;
}