using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientPublisher
    {
        Task PublishAsync(IConfiguration configuration, object file);
    }
}