using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalesAnalysis.FileGenerator.Application.DTO;
using SalesAnalysis.FileGenerator.Core.Interfaces;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileGenerator
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;

        private static IRabbitMqClientReceiver _clientReceiver;
        private static IOutputFileGenerator _fileGenerator;
        private static bool _firstTime = true;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(2000, stoppingToken);

                if (!_firstTime)
                    continue;

                var createScope = _serviceScopeFactory.CreateScope();

                _clientReceiver = createScope.ServiceProvider.GetRequiredService<IRabbitMqClientReceiver>();

                _fileGenerator = createScope.ServiceProvider.GetRequiredService<IOutputFileGenerator>();

                _clientReceiver.ConfigureChannel(_configuration["RabbitMqHostName"]
                   , _configuration["RabbitMqUsername"]
                   , _configuration["RabbitMqPassword"]
                   , int.Parse(_configuration["RabbitMqRetryCount"])
                   , _configuration["RabbitMqReceiveQueueName"]);

                _clientReceiver.Receive += (sender, args) =>
                {
                    var outputContentDto =
                        JsonConvert.DeserializeObject<OutputFileContentDto>(Encoding.UTF8.GetString(sender as byte[]));

                    if (outputContentDto == null)
                        return;

                    _fileGenerator.GenerateFIle(outputContentDto);
                };

                _firstTime = false;
            }
        }

        private void RabbitMqOnMessageReceived(object sender, EventArgs e)
        {
            var outputContentDto =
                JsonConvert.DeserializeObject<OutputFileContentDto>(Encoding.UTF8.GetString(sender as byte[]));

            if (outputContentDto == null)
                return;

            _fileGenerator.GenerateFIle(outputContentDto);

        }
    }
}
