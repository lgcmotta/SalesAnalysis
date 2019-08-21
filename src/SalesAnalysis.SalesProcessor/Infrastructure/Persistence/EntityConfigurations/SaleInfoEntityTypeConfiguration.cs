using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleInfoEntityTypeConfiguration : IEntityTypeConfiguration<SaleInfo>
    {
        public void Configure(EntityTypeBuilder<SaleInfo> builder)
        {
            builder.ToTable("SaleDetails");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.ItemId).IsRequired();
            builder.Property(s => s.ItemPrice).IsRequired();
            builder.Property(s => s.ItemQuantity).IsRequired();

        }
    }
}