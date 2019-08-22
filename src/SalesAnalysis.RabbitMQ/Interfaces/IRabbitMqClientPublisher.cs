using System.Threading.Tasks;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientPublisher
    {
        void Publish(object file, string hostName, string username, string password
            , int retryCount, string queueName);
    }
}