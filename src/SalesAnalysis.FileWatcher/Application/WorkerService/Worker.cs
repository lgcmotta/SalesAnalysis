using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileWatcher.Core.Interfaces;

namespace SalesAnalysis.FileWatcher.Application.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IFolderScanner _folderScanner;
        public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                //await Task.Delay(1000, stoppingToken);
                var createScope = _serviceScopeFactory.CreateScope();
                _folderScanner = createScope.ServiceProvider.GetRequiredService<IFolderScanner>();
                await _folderScanner.StartFolderScanAsync();
            }
        }
    }
}
