using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.FileWatcher.Application.Scanner;
using SalesAnalysis.FileWatcher.Application.Worker;
using SalesAnalysis.FileWatcher.Core.Scanner;
using SalesAnalysis.FileWatcher.Infrastructure.Migrations;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.FileWatcher.Infrastructure.Registrations;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.ServicesConfiguration.Configurations;

namespace SalesAnalysis.FileWatcher
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var configuration = ConfigurationFactory.GetConfiguration();

                var host = CreateHostBuilder(configuration, args).Build();

                var migrateDbContext = new MigrateDbContext(host.Services);

                migrateDbContext.PerformDbContextMigration();

                await host.RunAsync();

            }
            catch (Exception exception)
            {

            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddScoped<IFolderScanner, FolderScanner>();
                    services.AddScoped<IRabbitMqClientPublisher, RabbitMqClientPublisher>();
                    services.AddEntityFrameworkSqlServer()
                        .AddDbContext<FileWatcherDbContext>(options =>
                        {
                            options.UseSqlServer(configuration.GetConnectionString("WatcherConnectionString"),
                                sqlOptions => { sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });
                        });
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.AddAutoFacModules(configuration);
                });

    }
}
