using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.FileWriter.Infrastructure.Persistence;
using SalesAnalysis.ServicesConfiguration.Configurations;
using SalesAnalysis.FileWriter.Infrastructure.Extensions;
using SalesAnalysis.FileWriter.Infrastructure.Migrations;
using SalesAnalysis.FileWriter.Infrastructure.Registrations;

namespace SalesAnalysis.FileWriter
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
                    services.AddHostedService<Worker>();
                    services.AddSqlServerConfiguration(configuration);
                    services.AddAutoMapperConfiguration();
                    services.AddRabbitMqConfiguration();
                    services.AddOutputFileGeneratorConfiguration(configuration);
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => { /*builder.RegisterModule(new AutoFacRegistrations());*/ });
    }
}
