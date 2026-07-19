using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataTransferAndIntegrationSystem.Persistence.Repositories;

public class ErrorLogRepository : IErrorLogRepository
{
    private readonly AppDbContext _context;

    public ErrorLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ErrorLog>> GetAllAsync()
    {
        return await _context.ErrorLogs
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task AddAsync(ErrorLog errorLog)
    {
        await _context.ErrorLogs.AddAsync(errorLog);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}