using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.SalesProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        private IRabbitMqClientReceiver _clientReceiver;
        
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
            await _clientReceiver.ConfigureChannel(_configuration);
            
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
                
            }
        }
    }
}
