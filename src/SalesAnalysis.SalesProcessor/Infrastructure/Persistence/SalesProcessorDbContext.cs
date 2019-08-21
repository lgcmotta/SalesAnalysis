using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesAnalysis.SalesProcessor.Core.Domain;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence.EntityConfigurations;
using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Persistence
{
    public class SalesProcessorDbContext : DbContext, IUnitOfWork
    {
        private readonly DbContextOptions<SalesProcessorDbContext> _options;

        public SalesProcessorDbContext(DbContextOptions<SalesProcessorDbContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<InputFile> InputFiles { get; set; }

        public DbSet<Salesman> Salesmen { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Sale> Sales { get; set; }

        public DbSet<SaleInfo> SalesInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InputFIleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SalesmanEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new CustomerEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SaleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new SaleInfoEntityTypeConfiguration());
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