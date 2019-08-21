using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.RabbitMQ.Implementations
{
    public class RabbitMqClientReceiver : IRabbitMqClientReceiver
    {
        private readonly ILogger<RabbitMqClientReceiver> _logger;

        public RabbitMqClientReceiver(ILogger<RabbitMqClientReceiver> logger)
        {
            _logger = logger;
        }

        public async Task ConfigureChannel(IConfiguration configuration)
        {
            var retryCount = int.Parse(configuration["RabbitMqRetryCount"]);

            var policy = PolicyHelper.CreatePolicy(_logger, retryCount);

            policy.Execute(() =>
            {
                var factory = RabbitMqHelper.CreateConnectionFactory(configuration);

                using var connection = factory.CreateConnection();

                using var channel = connection.CreateModel();

                channel.QueueDeclare(configuration["RabbitMqQueueName"]
                    , false
                    , false
                    , false
                    , null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += OnConsumerOnReceived;

                var consumeTag = channel.BasicConsume(configuration["RabbitMqQueueName"], true, consumer);
            });
        }

        private void OnConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
            var body = args.Body;
        }
    }
}