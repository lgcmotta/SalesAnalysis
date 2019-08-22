using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.FileGenerator.Infrastructure.Extensions;
using SalesAnalysis.FileGenerator.Infrastructure.Migrations;
using SalesAnalysis.ServicesConfiguration.Configurations;

namespace SalesAnalysis.FileGenerator
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

            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSqlServerConfiguration(configuration);
                    services.AddRabbitMqConfiguration();
                    services.AddOutputFileGeneratorConfiguration(configuration);
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder => { /*builder.RegisterModule(new AutoFacRegistrations());*/ });
    }
}
