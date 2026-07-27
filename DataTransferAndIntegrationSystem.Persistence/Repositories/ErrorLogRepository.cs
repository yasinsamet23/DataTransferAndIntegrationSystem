using DataTransferAndIntegrationSystem.Application.Interfaces;
using DataTransferAndIntegrationSystem.Domain.Entities;
using EFCore.BulkExtensions;
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

    public async Task BulkInsertAsync(List<ErrorLog> errorLogs)
    {
        if (errorLogs.Count == 0)
            return;

        await _context.BulkInsertAsync(errorLogs);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
