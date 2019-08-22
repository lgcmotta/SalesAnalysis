using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.FileWatcher.Infrastructure.Extensions;
using SalesAnalysis.FileWatcher.Infrastructure.Migrations;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.ServicesConfiguration.Configurations;

namespace SalesAnalysis.FileWatcher
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var configuration = ConfigurationFactory.GetConfiguration();

                var host = CreateHostBuilder(configuration, args).Build();
                var migratedbContext = new MigrateDbContext(host.Services);
                migratedbContext.MigrateContext();

                host.Run();
            }
            catch (Exception exception)
            {
                Main(args);
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSqlServerConfiguration(configuration);
                    services.AddRabbitMqConfiguration();
                    services.AddFolderScannerConfiguration(configuration);
                })
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => { /*builder.RegisterModule(new AutoFacRegistrations());*/ });

    }
}
