using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using SalesAnalysis.FileWatcher.Core.Domain;
using SalesAnalysis.FileWatcher.Core.RabbitMQ;

namespace SalesAnalysis.FileWatcher.Application.RabbitMQ
{
    public class RabbitMqClientPublisher : IRabbitMqClientPublisher
    {
        private readonly ILogger<RabbitMqClientPublisher> _logger;
        private object _syncroot = new object();

        public RabbitMqClientPublisher(ILogger<RabbitMqClientPublisher> logger)
        {
            _logger = logger;
        }


        public async Task PublishAsync(IConfiguration configuration, InputFile file)
        {
            lock (_syncroot)
            {
                var retryCount = int.Parse(configuration["RabbitMqRetryCount"]);

                var policy = Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(retryCount
                    , retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                    , (ex, time) =>
                    {
                        _logger.LogWarning(ex, "RabbitMQ Client could not connect atver {TimeOut}s ({ExceptionMessage})"
                            , $"{time.TotalSeconds:n1}", ex.Message);
                    });

                policy.Execute(() =>
                {
                    var factory = new ConnectionFactory {HostName = configuration["RabbitMqHostName"]
                        ,UserName = configuration["RabbitMqUsername"]
                        ,Password = configuration["RabbitMqPassword"]
                    };

                    using var connection = factory.CreateConnection();

                    using var channel = connection.CreateModel();

                    channel.QueueDeclare(configuration["RabbitMqQueueName"]
                        , false
                        , false
                        , false
                        , null);
                    
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
