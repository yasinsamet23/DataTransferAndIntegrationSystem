using DataTransferAndIntegrationSystem.Application.DTOs;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IErrorLogService
{
    Task<List<ErrorLogDto>> GetAllErrorsAsync();

    Task AddErrorAsync(ErrorLogDto errorLogDto);
    Task BulkInsertErrorsAsync(List<ErrorLogDto> errorLogDtos);
}