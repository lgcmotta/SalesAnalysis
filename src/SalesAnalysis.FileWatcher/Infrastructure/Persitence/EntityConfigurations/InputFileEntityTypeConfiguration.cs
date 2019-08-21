using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SalesAnalysis.FileWatcher.Core.Domain;

namespace SalesAnalysis.FileWatcher.Infrastructure.Persitence.EntityConfigurations
{
    public class InputFileEntityTypeConfiguration : IEntityTypeConfiguration<InputFile>
    {
        public void Configure(EntityTypeBuilder<InputFile> builder)
        {
            builder.ToTable("InputProcessedFiles");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.FileName).IsRequired();

            builder.Property(p => p.FileExtension).IsRequired();

            builder.Property(p => p.FilePath).IsRequired();

            builder.Property(p => p.Processed).IsRequired();

            builder.Property(p => p.ProcessDate).IsRequired();
        }
    }
}