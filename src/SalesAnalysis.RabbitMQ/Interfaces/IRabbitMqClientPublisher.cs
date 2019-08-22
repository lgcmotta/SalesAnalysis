using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientPublisher
    {
        Task PublishAsync(object file, string hostName, string username, string password
            , int retryCount, string queueName);
    }
}