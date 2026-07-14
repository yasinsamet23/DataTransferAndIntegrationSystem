using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface ITransferLogService
{
    Task<List<TransferLogDto>> GetAllTransferLogsAsync();

    Task AddTransferLogAsync(TransferLogDto transferLogDto);
}