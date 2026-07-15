using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface IErrorLogRepository
{
    Task<List<ErrorLog>> GetAllAsync();

    Task AddAsync(ErrorLog errorLog);

    Task SaveChangesAsync();
}