using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Application.Services;

public class ErrorLogService : IErrorLogService
{
    private readonly IErrorLogRepository _errorLogRepository;

    public ErrorLogService(IErrorLogRepository errorLogRepository)
    {
        _errorLogRepository = errorLogRepository;
    }

    public async Task<List<ErrorLogDto>> GetAllErrorsAsync()
    {
        var errorLogs = await _errorLogRepository.GetAllAsync();

        return errorLogs.Select(error => new ErrorLogDto
        {
            Id = error.Id,
            RecordId = error.RecordId,
            ErrorField = error.ErrorField,
            ErrorMessage = error.ErrorMessage,
            CreatedDate = error.CreatedDate
        }).ToList();
    }

    public async Task AddErrorAsync(ErrorLogDto errorLogDto)
    {
        var errorLog = new ErrorLog
        {
            Id = Guid.NewGuid(),
            RecordId = errorLogDto.RecordId,
            ErrorField = errorLogDto.ErrorField,
            ErrorMessage = errorLogDto.ErrorMessage,
            CreatedDate = DateTime.UtcNow
        };

        await _errorLogRepository.AddAsync(errorLog);

        await _errorLogRepository.SaveChangesAsync();
    }
}