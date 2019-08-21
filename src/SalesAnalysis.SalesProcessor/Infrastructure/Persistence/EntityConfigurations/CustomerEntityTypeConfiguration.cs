using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations
{
    public class CustomerEntityTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).UseSqlServerIdentityColumn();
            builder.Property(c => c.Name).IsRequired();
            builder.Property(c => c.Cnpj).IsRequired();
            builder.Property(c => c.BusinessArea).IsRequired();
        }
    }
}