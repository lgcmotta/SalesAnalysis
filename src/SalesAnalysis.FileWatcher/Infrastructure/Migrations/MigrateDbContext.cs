using System;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;

namespace SalesAnalysis.FileWatcher.Infrastructure.Migrations
{
    public class MigrateDbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrateDbContext(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void MigrateContext()
        {
            using var serviceScope = _serviceProvider.GetService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<FileWatcherDbContext>();
            context.Database.EnsureCreated();
        }
    }
}