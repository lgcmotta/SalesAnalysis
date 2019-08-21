using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.Processors;
using SalesAnalysis.SalesProcessor.Application.Worker;
using SalesAnalysis.SalesProcessor.Core.Processors;
using SalesAnalysis.SalesProcessor.Infrastructure.Migrations;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;
using SalesAnalysis.ServicesConfiguration.Configurations;

namespace SalesAnalysis.SalesProcessor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                var configuration = ConfigurationFactory.GetConfiguration();

                var host = CreateHostBuilder(configuration,args).Build();

                var migrateDbContext = new MigrateDbContext(host.Services);
                migrateDbContext.PerformDbContextMigration();

                await  host.RunAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddEntityFrameworkSqlServer()
                        .AddDbContext<SalesProcessorDbContext>(options =>
                        {
                            options.UseSqlServer(configuration.GetConnectionString("ProcessorConnectionString"),
                                sqlOptions => { sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });
                        });
                    services.AddSingleton<IRabbitMqClientReceiver>(r =>
                    {
                        var logger = r.GetRequiredService<ILogger<RabbitMqClientReceiver>>();
                        var factory = RabbitMqHelper.CreateConnectionFactory(configuration);
                        return new RabbitMqClientReceiver(logger, factory, configuration);
                    });
                    services.AddSingleton<IRabbitMqClientPublisher>(r =>
                    {
                        var logger = r.GetRequiredService<ILogger<RabbitMqClientPublisher>>();
                        return new RabbitMqClientPublisher(logger);
                    });
                    services.AddSingleton<IDbProcessor>(s =>
                    {
                        var logger = s.GetRequiredService<ILogger<DbProcessor>>();
                        var context = s.GetRequiredService<SalesProcessorDbContext>();
                        var rabbit = s.GetRequiredService<IRabbitMqClientPublisher>();
                        return new DbProcessor(logger, context, rabbit);
                    });
                    services.AddSingleton<ISalesProcessor>(s =>
                    {
                        var logger = s.GetRequiredService<ILogger<SaleProcessor>>();
                        var dbProcessor = s.GetRequiredService<IDbProcessor>();
                        return new SaleProcessor(logger, configuration, dbProcessor);
                    });
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    //builder.RegisterType(typeof())
                });
    }
}
