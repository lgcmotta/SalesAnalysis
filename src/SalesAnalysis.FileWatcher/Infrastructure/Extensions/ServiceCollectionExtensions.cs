using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileWatcher.Application.BusinessLogic;
using SalesAnalysis.FileWatcher.Core.Interfaces;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWatcher.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqClientPublisher>(x =>
            {
                var logger = x.GetRequiredService<ILogger<RabbitMqClientPublisher>>();
                return new RabbitMqClientPublisher(logger);
            });
            return services;
        }

        public static IServiceCollection AddFolderScannerConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddSingleton<IFolderScanner>(x =>
            {
                var logger = x.GetRequiredService<ILogger<FolderScanner>>();
                var context = x.GetRequiredService<FileWatcherDbContext>();
                var rabbit = x.GetRequiredService<IRabbitMqClientPublisher>();
                return new FolderScanner(logger, configuration, rabbit, context);
            });
            return services;
        }

        public static IServiceCollection AddSqlServerConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<FileWatcherDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("WatcherConnectionString"),
                        sqlOptions => { sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });
                });

            return services;
        }
    }
}