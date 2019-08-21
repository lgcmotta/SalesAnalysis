using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SalesAnalysis.FileWatcher.Core.Domain;
using SalesAnalysis.FileWatcher.Core.RabbitMQ;
using SalesAnalysis.FileWatcher.Core.Scanner;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;

namespace SalesAnalysis.FileWatcher.Application.Scanner
{
    public class FolderScanner : IFolderScanner
    {
        private readonly FileWatcherDbContext _context;
        private string _folderPath;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FolderScanner(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, FileWatcherDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _folderPath = configuration["InputFolder"];
        }

        public async Task StartFolderScanAsync()
        {
            var files = _context.InputFiles.Where(f => !f.Processed);

            var filesInFolder = new DirectoryInfo(_folderPath).GetFiles();

            foreach (var fileInfo in filesInFolder)
            {
                var file = new InputFile
                {
                    FileName = fileInfo.Name
                    , FileExtension = fileInfo.Extension
                    , FilePath = fileInfo.Directory.FullName
                    , Processed = true
                    , ProcessDate = DateTime.Now
                };

                await _context.InputFiles.AddAsync(file);

                var createScope = _serviceScopeFactory.CreateScope();

                var rabbitMqClientPublisher = createScope.ServiceProvider.GetRequiredService<IRabbitMqClientPublisher>();

                await rabbitMqClientPublisher.PublishAsync(_configuration, file);
                
            }

            var saved = await _context.SaveAsync();
        }
    }
}