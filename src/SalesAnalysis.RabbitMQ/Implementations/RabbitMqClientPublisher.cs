using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using SalesAnalysis.RabbitMQ.Helpers;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.RabbitMQ.Implementations
{
    public class RabbitMqClientPublisher : IRabbitMqClientPublisher
    {
        private readonly ILogger<RabbitMqClientPublisher> _logger;
        private object _syncroot = new object();

        public RabbitMqClientPublisher(ILogger<RabbitMqClientPublisher> logger)
        {
            _logger = logger;
        }
        
        public void Publish(object file, string hostName, string username, string password, int retryCount, string queueName)
        {
            lock (_syncroot)
            {
                _logger.LogInformation("RabbitMQ is trying to connect.");

                var policy = PolicyHelper.CreateRabbitMqPolicy(_logger,retryCount);

                policy.Execute(() =>
                {
                    var factory = RabbitMqHelper.CreateConnectionFactory(hostName,username,password);

                    using var connection = factory.CreateConnection();

                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(queue: queueName,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(file));

                    var properties = channel.CreateBasicProperties();

                    properties.Persistent = true;

                    channel.BasicPublish(""
                        ,queueName
                        , properties
                        , body);

                    _logger.LogInformation("RabbitMQ client is ready do publish messages.");

                });
            }
        }
    }
}
