using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileGenerator.Application.BusinessLogic;
using SalesAnalysis.FileGenerator.Application.DTO;
using SalesAnalysis.FileGenerator.Core.Domain;
using SalesAnalysis.FileGenerator.Core.Interfaces;
using SalesAnalysis.FileGenerator.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileGenerator.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<FileWriterDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("WriterConnectionString"),
                        sqlOptions => { sqlOptions.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null); });
                });
            return services;
        }

        public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services)
        {
            services.AddScoped<IRabbitMqClientReceiver>(r =>
            {
                var logger = r.GetRequiredService<ILogger<RabbitMqClientReceiver>>();
                return new RabbitMqClientReceiver(logger);
            });
            return services;
        }

        public static IServiceCollection AddOutputFileGeneratorConfiguration(this IServiceCollection services
            , IConfiguration configuration)
        {
            services.AddSingleton<IOutputFileGenerator>(o =>
            {
                var logger = o.GetRequiredService<ILogger<OutputFileGenerator>>();
                var context = o.GetRequiredService<FileWriterDbContext>();
                return new OutputFileGenerator(logger, configuration, context);
            });
            return services;
        }
    }
}