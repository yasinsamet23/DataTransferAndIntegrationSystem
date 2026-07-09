using DataTransferAndIntegrationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataTransferAndIntegrationSystem.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<TransferLog> TransferLogs { get; set; }

    public DbSet<ErrorLog> ErrorLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}