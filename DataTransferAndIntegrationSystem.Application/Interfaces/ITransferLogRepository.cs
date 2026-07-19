using DataTransferAndIntegrationSystem.Domain.Entities;

namespace DataTransferAndIntegrationSystem.Application.Interfaces;

public interface ITransferLogRepository
{
    Task<List<TransferLog>> GetAllAsync();

    Task AddAsync(TransferLog transferLog);

    Task SaveChangesAsync();

    Task<TransferLog?> GetByIdAsync(Guid id);
}