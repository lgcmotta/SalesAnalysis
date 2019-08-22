using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
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
        
        public async Task PublishAsync(object file, string hostName, string username, string password, int retryCount, string queueName)
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

                    channel.QueueDeclare(queueName
                        , false
                        , false
                        , false
                        , null);

                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(file));

                    channel.BasicPublish(""
                        , queueName
                        , null
                        , body);

                    _logger.LogInformation("RabbitMQ client is ready do publish messages.");

                });
            }
        }
    }
}
