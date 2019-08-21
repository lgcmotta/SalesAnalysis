using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
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
        
        public async Task PublishAsync(IConfiguration configuration, object file)
        {
            lock (_syncroot)
            {
                var retryCount = int.Parse(configuration["RabbitMqRetryCount"]);

                var policy = PolicyHelper.CreatePolicy(_logger,retryCount);

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

                    //var body = Encoding.GetEncoding("iso-8859-1").GetBytes(JsonConvert.SerializeObject(file));
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(file));

                    channel.BasicPublish(""
                        , configuration["RabbitMqQueueName"]
                        , null
                        , body);
                });
            }
        }
    }
}
