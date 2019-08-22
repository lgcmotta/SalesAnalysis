using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SalesAnalysis.RabbitMQ.EventArgs;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientReceiver
    {
        Task ConfigureChannel(string hostName, string username, string password
            , int retryCount, string queueName);

        event EventHandler Receive;

        void OnReceived(object sender);
    }
}