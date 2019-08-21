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
        private readonly ConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;

        private IConnection _connection;
        private IModel _channel;
        private EventingBasicConsumer _consumer;

        public event EventHandler Receive;


        public RabbitMqClientReceiver(ILogger<RabbitMqClientReceiver> logger, ConnectionFactory connectionFactory, IConfiguration configuration)
        {
            _logger = logger;
            _connectionFactory = connectionFactory;
            _configuration = configuration;
        }

        public async Task ConfigureChannel()
        {
            var retryCount = int.Parse(_configuration["RabbitMqRetryCount"]);

            var policy = PolicyHelper.CreateRabbitMqPolicy(_logger, retryCount);

            policy.Execute(() =>
            {
                _connection = _connectionFactory.CreateConnection();

                _channel = _connection.CreateModel();

                _channel.QueueDeclare(_configuration["RabbitMqQueueName"]
                    , false
                    , false
                    , false
                    , null);

                _consumer = new EventingBasicConsumer(_channel);

                _consumer.Received += OnConsumerOnReceived;

                _channel.BasicConsume(_configuration["RabbitMqQueueName"], true, _consumer);
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