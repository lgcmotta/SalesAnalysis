using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SalesAnalysis.RabbitMQ.Helpers;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.RabbitMQ.Implementations
{
    public class RabbitMqClientReceiver : IRabbitMqClientReceiver
    {
        private readonly ILogger<RabbitMqClientReceiver> _logger;

        private static ConnectionFactory _connectionFactory;
        private static IConnection _connection;
        private static EventingBasicConsumer _consumer;
        private static IModel _channel;

        public event EventHandler Receive;

        public RabbitMqClientReceiver(ILogger<RabbitMqClientReceiver> logger)
        {
            _logger = logger;
        }

        public void ConfigureChannel(string hostName, string username, string password, int retryCount, string queueName)
        {
            _logger.LogInformation("RabbitMQ is trying to connect");

            var policy = PolicyHelper.CreateRabbitMqPolicy(_logger, retryCount);

            policy.Execute(() =>
            {
                _connectionFactory = new ConnectionFactory()
                {
                    HostName = hostName
                    , UserName = username
                    , Password = password
                };

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