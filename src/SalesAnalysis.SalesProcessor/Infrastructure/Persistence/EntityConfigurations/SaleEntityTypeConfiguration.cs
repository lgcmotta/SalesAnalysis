using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleEntityTypeConfiguration : IEntityTypeConfiguration<Sale>
    {
        public void Configure(EntityTypeBuilder<Sale> builder)
        {
            builder.ToTable("Sales");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id).UseSqlServerIdentityColumn();

            builder.Property(s => s.InputFile).IsRequired();

            builder.HasOne(s => s.InputFile);

            builder.Property(s => s.SaleId).IsRequired();

            builder.Property(s => s.SalesmanName).IsRequired();

            builder.HasMany(s => s.SaleInfo);

        }
    }
}