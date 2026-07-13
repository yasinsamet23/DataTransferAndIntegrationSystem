using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface ITransferService
{
    Task<TransferResultDto> StartTransferAsync();
}