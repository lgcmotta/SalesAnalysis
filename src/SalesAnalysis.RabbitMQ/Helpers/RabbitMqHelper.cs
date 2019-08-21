using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SalesAnalysis.RabbitMQ.Helpers
{
    public static class RabbitMqHelper
    {
        public static ConnectionFactory CreateConnectionFactory(IConfiguration configuration) =>
            new ConnectionFactory
            {
                HostName = configuration["RabbitMqHostName"]
                , UserName = configuration["RabbitMqUsername"]
                , Password = configuration["RabbitMqPassword"]
            };

    }
}