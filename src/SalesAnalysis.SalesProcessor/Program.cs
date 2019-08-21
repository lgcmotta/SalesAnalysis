using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
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
                    services.AddScoped<IRabbitMqClientReceiver, RabbitMqClientReceiver>();
                }).UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(builder =>
                {
                    //builder.RegisterType(typeof())
                });
    }
}
