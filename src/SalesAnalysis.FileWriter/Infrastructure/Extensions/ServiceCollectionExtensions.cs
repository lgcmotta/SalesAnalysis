using System;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileWriter.Application.BusinessLogic;
using SalesAnalysis.FileWriter.Application.DTO;
using SalesAnalysis.FileWriter.Core.Domain;
using SalesAnalysis.FileWriter.Core.Interfaces;
using SalesAnalysis.FileWriter.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Implementations;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWriter.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutoMapperConfiguration(this IServiceCollection services)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<OutputFileContentDto, OutputFileContent>()
                    .ForMember(x => x.Id, x => x.Ignore());
                cfg.CreateMap<OutputFileContent, OutputFileContentDto>();
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            return services;
        }

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
            services.AddSingleton<IRabbitMqClientReceiver>(r =>
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
                var mapper = o.GetRequiredService<IMapper>();
                var context = o.GetRequiredService<FileWriterDbContext>();
                return new OutputFileGenerator(logger, configuration, mapper, context);
            });
            return services;
        }
    }
}