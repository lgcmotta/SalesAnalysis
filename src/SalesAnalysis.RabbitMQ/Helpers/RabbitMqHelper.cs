using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace SalesAnalysis.RabbitMQ.Helpers
{
    public static class RabbitMqHelper
    {
        public static ConnectionFactory CreateConnectionFactory(string hostname, string username, string password) =>
            new ConnectionFactory
            {
                HostName = hostname
                , UserName = username
                , Password = password
            };

    }
}