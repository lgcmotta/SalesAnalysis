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

namespace SalesAnalysis.SalesProcessor.Application.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private static IRabbitMqClientReceiver _clientReceiver;
        private static ISalesFileAnalyser _salesFileAnalyser;
        
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var createScope = _serviceScopeFactory.CreateScope();

            _clientReceiver = createScope.ServiceProvider.GetRequiredService<IRabbitMqClientReceiver>();
            _salesFileAnalyser = createScope.ServiceProvider.GetRequiredService<ISalesFileAnalyser>();
            await _clientReceiver.ConfigureChannel();

            _clientReceiver.Receive += RabbitMqClientOnRecieve;
        }

        private void RabbitMqClientOnRecieve(object sender, EventArgs e)
        {
            //var file = JsonConvert.DeserializeObject<InputFile>(Encoding.GetEncoding("iso-8859-1").GetString(sender as byte[]));
            var file = JsonConvert.DeserializeObject<InputFile>(Encoding.UTF8.GetString(sender as byte[]));

            _salesFileAnalyser.ProcessInputFile(file).GetAwaiter();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Sales processor running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
