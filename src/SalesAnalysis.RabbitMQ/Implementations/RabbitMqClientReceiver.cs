using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesAnalysis.RabbitMQ.EventArgs;
using SalesAnalysis.RabbitMQ.Helpers;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.RabbitMQ.Implementations
{
    public class RabbitMqClientReceiver : IRabbitMqClientReceiver
    {
        private readonly ILogger<RabbitMqClientReceiver> _logger;

        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public event EventHandler Receive;

        public RabbitMqClientReceiver(ILogger<RabbitMqClientReceiver> logger)
        {
            _logger = logger;
        }

        public async Task ConfigureChannel(string hostName, string username, string password, int retryCount, string queueName)
        {
            _logger.LogInformation("RabbitMQ is trying to connect");

            var policy = PolicyHelper.CreateRabbitMqPolicy(_logger, retryCount);

            policy.Execute(() =>
            {
                _connectionFactory = RabbitMqHelper.CreateConnectionFactory(hostName, username, password);

                _connection = _connectionFactory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.QueueDeclare(queueName
                    , false
                    , false
                    , false
                    , null);

                _consumer = new EventingBasicConsumer(_channel);

                _consumer.Received += OnConsumerOnReceived;

                _channel.BasicConsume(queueName, true, _consumer);

                _logger.LogInformation("RabbitMQ Client is ready to receive messages");
            });
        }

        
        public void OnReceived(object sender)
        {
            Receive?.Invoke(sender,System.EventArgs.Empty);
        }
        
        private void OnConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
            OnReceived(args.Body);
        }
    }
}