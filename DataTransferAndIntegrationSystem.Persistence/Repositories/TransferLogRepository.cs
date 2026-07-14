using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataTransferAndIntegrationSystem.Persistence.Repositories;

public class TransferLogRepository : ITransferLogRepository
{
    private readonly AppDbContext _context;

    public TransferLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TransferLog>> GetAllAsync()
    {
        return await _context.TransferLogs.ToListAsync();
    }

    public async Task AddAsync(TransferLog transferLog)
    {
        await _context.TransferLogs.AddAsync(transferLog);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}