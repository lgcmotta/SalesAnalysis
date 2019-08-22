using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.DTO;
using SalesAnalysis.SalesProcessor.Application.Extensions;
using SalesAnalysis.SalesProcessor.Core.Interfaces;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Application.BusinessLogic
{
    public class OutputDataProcessor : IOutputDataProcessor
    {
        private readonly ILogger<OutputDataProcessor> _logger;
        private readonly IRabbitMqClientPublisher _clientPublisher;
        private readonly IConfiguration _configuration;
        private readonly SalesProcessorDbContext _context;

        private OutputDataDto _outputData;

        public OutputDataProcessor(ILogger<OutputDataProcessor> logger, IRabbitMqClientPublisher clientPublisher, IConfiguration configuration, SalesProcessorDbContext context)
        {
            _logger = logger;
            _clientPublisher = clientPublisher;
            _configuration = configuration;
            _context = context;
        }

        public async Task BuildOutputData(FileContentDto fileContent)
        {
            try
            {
                _outputData = new OutputDataDto()
                    .GetCustomersQuantity(fileContent)
                    .GetSalesmenQuantity(fileContent)
                    .GetIdFromMostExpensiveSale(fileContent, _context)
                    .GetWorstSalesman(fileContent);

                await _clientPublisher.PublishAsync(_outputData
                , _configuration["RabbitMqHostName"]
                , _configuration["RabbitMqUsername"]
                , _configuration["RabbitMqPassword"]
                ,int.Parse(_configuration["RabbitMqRetryCount"])
                , _configuration["RabbitMqPublishQueueName"]);
            }
            catch (Exception exception)
            {
                _logger.LogCritical("An unexpected exception occurred when building output data object");
                _logger.LogCritical("Exception: {message}",exception.Message);
                _logger.LogTrace(exception.Message);
            }

        }
    }
}