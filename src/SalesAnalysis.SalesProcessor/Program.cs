using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Infrastructure.Extensions;
using SalesAnalysis.SalesProcessor.Infrastructure.Migrations;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;
using SalesAnalysis.SalesProcessor.Infrastructure.Registrations;
using SalesAnalysis.ServicesConfiguration.Configurations;

namespace SalesAnalysis.SalesProcessor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var configuration = ConfigurationFactory.GetConfiguration();

                var host = CreateHostBuilder(configuration,args).Build();
                var migratedbContext = new MigrateDbContext(host.Services);
                migratedbContext.MigrateContext();

                host.Run();
            }
            catch (Exception exception)
            {
    
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<SalesProcessorWorker>();
                    services.AddSqlServerConfiguration(configuration);
                    services.AddRabbitMqPublisherConfiguration();
                    services.AddRabbitMqReceieverConfiguration();
                    services.AddOutputDataProcessorConfiguration(configuration);
                    services.AddSalesDataProcessorConfiguration(configuration);
                    services.AddSalesFileAnalyserConfiguration(configuration);
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    builder.RegisterType(typeof(SalesProcessorDbContext));
                });
    }
}
