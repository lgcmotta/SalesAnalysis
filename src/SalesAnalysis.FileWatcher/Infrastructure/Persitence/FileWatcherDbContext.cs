using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesAnalysis.FileWatcher.Core.Domain;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence.EntityConfigurations;
using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.FileWatcher.Infrastructure.Persitence
{
    public class FileWatcherDbContext : DbContext, IUnitOfWork
    {
        private DbContextOptions<FileWatcherDbContext> _options;

        public FileWatcherDbContext(DbContextOptions<FileWatcherDbContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<InputFile> InputFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InputFileEntityTypeConfiguration());
        }

        public int Save()
        {
            return SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await SaveChangesAsync();
        }
    }
}