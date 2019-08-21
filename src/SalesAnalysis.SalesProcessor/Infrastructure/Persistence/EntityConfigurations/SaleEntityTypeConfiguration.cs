using System;
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
            
            builder.Property(s => s.SaleId).IsRequired();

            builder.Property(s => s.SalesmanName).IsRequired();

            builder.Property(s => s.InputFileName).IsRequired();

            builder.HasMany(p => p.SalesInfo)
                .WithOne(p => p.Sale)
                .HasForeignKey(f => f.FkSale);

        }
    }
}