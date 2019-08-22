using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileWatcher.Core.Domain;
using SalesAnalysis.FileWatcher.Core.Interfaces;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.RabbitMQ.Helpers;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWatcher.Application.BusinessLogic
{
    public class FolderScanner : IFolderScanner
    {
        private readonly ILogger<FolderScanner> _logger;
        private readonly FileWatcherDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqClientPublisher _publisher;
        private string _folderPath;


        public FolderScanner(ILogger<FolderScanner> logger, IConfiguration configuration, IRabbitMqClientPublisher publisher, FileWatcherDbContext context)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _publisher = publisher;
            _folderPath = configuration["InputFolder"];
        }

        public void StartFolderScanAsync()
        {
            var policy = PolicyHelper.CreateSqlPolicy(_logger, 3);

            policy.Execute(ScanFolder);
        }

        private void ScanFolder()
        {
            var extensions = GetExtensions();

            var files =  _context.InputFiles.ToList();

            var filesInFolder = new DirectoryInfo(_folderPath).GetFiles();

            foreach (var fileInfo in filesInFolder)
            {
                if(!extensions.Any(e => e.Equals(fileInfo.Extension)))
                    continue;

                if (files.Any(f => f.FileName == fileInfo.Name 
                                   && f.FileExtension == fileInfo.Extension 
                                   && f.Processed))
                    continue;

                var file = CreateInputFileEntity(fileInfo);

                 _context.InputFiles.Add(file);

                 _publisher.Publish(file
                    , _configuration["RabbitMqHostName"]
                    , _configuration["RabbitMqUsername"]
                    , _configuration["RabbitMqPassword"]
                    ,int.Parse(_configuration["RabbitMqRetryCount"])
                    ,_configuration["RabbitMqPublishQueueName"]);
            }

            var saved =  _context.SaveAsync();
        }

        private List<string> GetExtensions() => _configuration.GetSection("ExtensionsToScan").GetChildren().Select(x => x.Value).ToList();

        private static InputFile CreateInputFileEntity(FileInfo fileInfo) =>
            new InputFile
            {
                FileName = fileInfo.Name
                , FileExtension = fileInfo.Extension
                , FilePath = fileInfo.Directory.FullName
                , Processed = true
                , ProcessDate = DateTime.Now
            };

     
    }
}