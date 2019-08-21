using System;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Migrations
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
            var context = serviceScope.ServiceProvider.GetRequiredService<SalesProcessorDbContext>();
            context.Database.EnsureCreated();
        }
    }
}
