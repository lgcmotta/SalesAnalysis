using Microsoft.EntityFrameworkCore;
using SalesAnalysis.FileGenerator.Core.Domain;
using SalesAnalysis.FileGenerator.Infrastructure.Persistence.EntityConfigurations;

namespace SalesAnalysis.FileGenerator.Infrastructure.Persistence
{
    public class FileWriterDbContext : DbContext
    {
        private readonly DbContextOptions<FileWriterDbContext> _options;

        public FileWriterDbContext(DbContextOptions<FileWriterDbContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<OutputFileContent> OutputFilesContent { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OutputFileContentEntityTypeConfiguration());
        }
    }
}