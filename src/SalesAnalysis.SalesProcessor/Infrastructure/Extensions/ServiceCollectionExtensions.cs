using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.BusinessLogic;
using SalesAnalysis.SalesProcessor.Core.Interfaces;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<SalesProcessorDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("ProcessorConnectionString"),
                        sqlOptions => { sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });
                });
            return services;
        }

        public static IServiceCollection AddRabbitMqReceieverConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqClientReceiver>(r =>
            {
                var logger = r.GetRequiredService<ILogger<RabbitMqClientReceiver>>();
                return new RabbitMqClientReceiver(logger);
            });

            return services;
        }

        public static IServiceCollection AddRabbitMqPublisherConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqClientPublisher>(r =>
            {
                var logger = r.GetRequiredService<ILogger<RabbitMqClientPublisher>>();
                return new RabbitMqClientPublisher(logger);
            });
            return services;
        }

        public static IServiceCollection AddOutputDataProcessorConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddSingleton<IOutputDataProcessor>(s =>
            {
                var logger = s.GetRequiredService<ILogger<OutputDataProcessor>>();
                var context = s.GetRequiredService<SalesProcessorDbContext>();
                var rabbit = s.GetRequiredService<IRabbitMqClientPublisher>();
                return new OutputDataProcessor(logger, rabbit, configuration, context);
            });
            return services;
        }

        public static IServiceCollection AddSalesDataProcessorConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddSingleton<ISalesDataProcessor>(s =>
            {
                var logger = s.GetRequiredService<ILogger<SalesDataProcessor>>();
                var context = s.GetRequiredService<SalesProcessorDbContext>();
                var outputProcessor = s.GetRequiredService<IOutputDataProcessor>();
                return new SalesDataProcessor(logger, context, outputProcessor);
            });
            return services;
        }

        public static IServiceCollection AddSalesFileAnalyserConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddSingleton<ISalesFileAnalyser>(s =>
            {
                var logger = s.GetRequiredService<ILogger<SalesFileAnalyzer>>();
                var dbProcessor = s.GetRequiredService<ISalesDataProcessor>();
                var rabbit = s.GetRequiredService<IRabbitMqClientPublisher>();
                return new SalesFileAnalyzer(logger, configuration, dbProcessor, rabbit);
            });
            return services;
        }
    }
}