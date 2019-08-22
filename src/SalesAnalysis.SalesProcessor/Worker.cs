using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Core.Domain;
using SalesAnalysis.SalesProcessor.Core.Interfaces;

namespace SalesAnalysis.SalesProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private static IRabbitMqClientReceiver _clientReceiver;
        private static ISalesFileAnalyser _salesFileAnalyser;
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
                //await Task.Delay(5000, stoppingToken);

                _logger.LogInformation("Sales processor running at: {time}", DateTimeOffset.Now);

                if (!_firstTime)
                    continue;

                var createScope = _serviceScopeFactory.CreateScope();

                _clientReceiver = createScope.ServiceProvider.GetRequiredService<IRabbitMqClientReceiver>();

                _salesFileAnalyser = createScope.ServiceProvider.GetRequiredService<ISalesFileAnalyser>();

                _clientReceiver.ConfigureChannel(_configuration["RabbitMqHostName"]
                    , _configuration["RabbitMqUsername"]
                    , _configuration["RabbitMqPassword"]
                    , int.Parse(_configuration["RabbitMqRetryCount"])
                    , _configuration["RabbitMqReceiveQueueName"]);

                _clientReceiver.Receive += RabbitMqClientMessageReceived;

                _firstTime = false;

            }
        }

        private void RabbitMqClientMessageReceived(object sender, EventArgs e)
        {
            var file = JsonConvert.DeserializeObject<InputFile>(Encoding.UTF8.GetString(sender as byte[]));

            _salesFileAnalyser.ProcessInputFile(file);
        }

    }
}
