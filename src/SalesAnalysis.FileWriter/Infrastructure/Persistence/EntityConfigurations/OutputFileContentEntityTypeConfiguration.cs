using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.FileWriter.Core.Domain;

namespace SalesAnalysis.FileWriter.Infrastructure.Persistence.EntityConfigurations
{
    public class OutputFileContentEntityTypeConfiguration : IEntityTypeConfiguration<OutputFileContent>
    {
        public void Configure(EntityTypeBuilder<OutputFileContent> builder)
        {
            builder.ToTable("OutputFilesContent");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.FileName).IsRequired();
            builder.Property(o => o.FileExtension).IsRequired();
            builder.Property(o => o.SalesmenQuantity).IsRequired();
            builder.Property(o => o.CustomersQuantity).IsRequired();
            builder.Property(o => o.MostExpensiveSale).HasColumnName("MostExpensiveSaleId").IsRequired();
            builder.Property(o => o.WorstSalesman).IsRequired();
            builder.Property(o => o.GenerationDate).IsRequired();
        }
    }
}