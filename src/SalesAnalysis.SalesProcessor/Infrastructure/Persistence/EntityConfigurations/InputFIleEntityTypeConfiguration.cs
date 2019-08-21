using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations
{
    public class InputFIleEntityTypeConfiguration : IEntityTypeConfiguration<InputFile>
    {
        public void Configure(EntityTypeBuilder<InputFile> builder)
        {
            builder.ToTable("ProcessedFiles");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Id).UseSqlServerIdentityColumn();

            builder.Property(f => f.FileName).IsRequired();

            builder.Property(f => f.FileExtension).IsRequired();

            builder.Property(f => f.FilePath).IsRequired();

            builder.Property(f => f.Processed).IsRequired();

            builder.Property(f => f.ProcessDate).IsRequired();

            builder.Property(f => f.Canceled).IsRequired();

        }
    }
}