using System;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;

namespace SalesAnalysis.FileWatcher.Infrastructure.Migrations
{
    public class MigrateDbContext
    {
        private readonly IServiceProvider serviceProvider;

        public MigrateDbContext(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void PerformDbContextMigration()
        {
            using var serviceScope = this.serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<FileWatcherDbContext>();
            context.Database.EnsureCreated();
        }
    }
}
