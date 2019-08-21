using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using SalesAnalysis.FileWatcher.Core.Domain;
using SalesAnalysis.FileWatcher.Core.Scanner;
using SalesAnalysis.FileWatcher.Infrastructure.Persitence;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWatcher.Application.Scanner
{
    public class FolderScanner : IFolderScanner
    {
        private readonly ILogger<FolderScanner> _logger;
        private readonly FileWatcherDbContext _context;
        private string _folderPath;
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public FolderScanner(ILogger<FolderScanner> logger, IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, FileWatcherDbContext context)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
            _folderPath = configuration["InputFolder"];
        }

        public async Task StartFolderScanAsync()
        {
            var policy = CreatePolicy();

            await policy.Execute(ScanFolder);
        }

        private async Task ScanFolder()
        {
            var extensions = GetExtensions();

            var files = await _context.InputFiles.ToListAsync();

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

                await _context.InputFiles.AddAsync(file);

                var createScope = _serviceScopeFactory.CreateScope();

                var rabbitMqClientPublisher = createScope.ServiceProvider.GetRequiredService<IRabbitMqClientPublisher>();

                await rabbitMqClientPublisher.PublishAsync(_configuration, file);
            }

            var saved = await _context.SaveAsync();
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

        private RetryPolicy CreatePolicy() =>
            Policy.Handle<SqlException>()
                .WaitAndRetry(3, retry => TimeSpan.FromSeconds(5)
                    , (exception, timeSpan, retry, ctx) =>
                    {
                        _logger.LogWarning(exception
                            , "Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}"
                            , exception.GetType().Name, exception.Message, retry, 3);
                    });
    }
}