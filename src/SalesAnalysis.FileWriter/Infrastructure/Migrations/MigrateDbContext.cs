using System;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.FileWriter.Infrastructure.Persistence;

namespace SalesAnalysis.FileWriter.Infrastructure.Migrations
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
            var context = serviceScope.ServiceProvider.GetRequiredService<FileWriterDbContext>();
            context.Database.EnsureCreated();
        }
    }
}