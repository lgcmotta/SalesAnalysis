using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations
{
    public class SalesmanEntityTypeConfiguration : IEntityTypeConfiguration<Salesman>
    {
        public void Configure(EntityTypeBuilder<Salesman> builder)
        {
            builder.ToTable("Salesmen");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).UseSqlServerIdentityColumn();
            builder.Property(s => s.Name).IsRequired();
            builder.Property(s => s.Cpf).IsRequired();
            builder.Property(s => s.Salary).IsRequired();
            
        }
    }
}