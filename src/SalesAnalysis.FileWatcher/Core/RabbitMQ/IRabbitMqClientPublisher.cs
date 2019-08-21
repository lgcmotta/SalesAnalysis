using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SalesAnalysis.FileWatcher.Core.Domain;

namespace SalesAnalysis.FileWatcher.Core.RabbitMQ
{
    public interface IRabbitMqClientPublisher
    {
        Task PublishAsync(IConfiguration configuration, InputFile file);
    }
}