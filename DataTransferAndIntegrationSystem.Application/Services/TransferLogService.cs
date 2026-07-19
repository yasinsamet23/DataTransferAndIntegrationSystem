using DataTransferAndIntegrationSystem.Application.DTOs;
using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Application.Services;

public class TransferLogService : ITransferLogService
{
    private readonly ITransferLogRepository _transferLogRepository;

    public TransferLogService(ITransferLogRepository transferLogRepository)
    {
        _transferLogRepository = transferLogRepository;
    }

    public async Task<List<TransferLogDto>> GetAllTransferLogsAsync()
    {
        var transferLogs = await _transferLogRepository.GetAllAsync();

        return transferLogs.Select(log => new TransferLogDto
        {
            Id = log.Id,
            TransferDate = log.TransferDate,
            TotalRecords = log.TotalRecords,
            SuccessCount = log.SuccessCount,
            Status = log.Status
        }).ToList();
    }

    public async Task AddTransferLogAsync(TransferLogDto transferLogDto)
    {
        var transferLog = new TransferLog
        {
            Id = transferLogDto.Id,
            TransferDate = transferLogDto.TransferDate,
            TotalRecords = transferLogDto.TotalRecords,
            SuccessCount = transferLogDto.SuccessCount,
            Status = transferLogDto.Status
        };

        await _transferLogRepository.AddAsync(transferLog);

        await _transferLogRepository.SaveChangesAsync();
    }

    public async Task UpdateTransferLogAsync(TransferLogDto transferLogDto)
    {
        var transferLog =
            await _transferLogRepository.GetByIdAsync(transferLogDto.Id);

        if (transferLog == null)
            throw new Exception("Transfer log not found.");

        transferLog.TransferDate = transferLogDto.TransferDate;
        transferLog.TotalRecords = transferLogDto.TotalRecords;
        transferLog.SuccessCount = transferLogDto.SuccessCount;
        transferLog.Status = transferLogDto.Status;

        await _transferLogRepository.SaveChangesAsync();
    }
}