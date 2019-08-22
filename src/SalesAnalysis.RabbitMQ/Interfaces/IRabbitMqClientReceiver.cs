using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientReceiver
    {
        void  ConfigureChannel(string hostName, string username, string password
            , int retryCount, string queueName);

        event EventHandler Receive;

        void OnReceived(object sender);
    }
}