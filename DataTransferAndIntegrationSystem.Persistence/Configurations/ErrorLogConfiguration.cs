using DataTransferAndIntegrationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataTransferAndIntegrationSystem.Persistence.Configurations;

public class ErrorLogConfiguration : IEntityTypeConfiguration<ErrorLog>
{
    public void Configure(EntityTypeBuilder<ErrorLog> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.RecordId)
               .IsRequired();

        builder.Property(x => x.ErrorField)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(x => x.ErrorMessage)
               .IsRequired()
               .HasMaxLength(500);

        builder.Property(x => x.CreatedDate)
               .IsRequired();
    }
}