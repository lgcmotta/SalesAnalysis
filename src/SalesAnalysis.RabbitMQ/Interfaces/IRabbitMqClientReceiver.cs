using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SalesAnalysis.RabbitMQ.Interfaces
{
    public interface IRabbitMqClientReceiver
    {
        Task ConfigureChannel(IConfiguration configuration);

    }
}