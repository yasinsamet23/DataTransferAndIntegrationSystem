using DataTransferAndIntegrationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataTransferAndIntegrationSystem.Persistence.Configurations;

public class TransferLogConfiguration : IEntityTypeConfiguration<TransferLog>
{
    public void Configure(EntityTypeBuilder<TransferLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransferDate)
               .IsRequired();

        builder.Property(x => x.TotalRecords)
               .IsRequired();

        builder.Property(x => x.SuccessCount)
               .IsRequired();

        builder.Property(x => x.Status)
               .IsRequired()
               .HasMaxLength(50);
    }
}