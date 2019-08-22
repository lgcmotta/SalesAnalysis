using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using SalesAnalysis.FileGenerator.Application.DTO;
using SalesAnalysis.FileGenerator.Core.Domain;
using SalesAnalysis.FileGenerator.Core.Interfaces;
using SalesAnalysis.FileGenerator.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileGenerator.Application.BusinessLogic
{
    public class OutputFileGenerator : IOutputFileGenerator
    {
        private readonly ILogger<OutputFileGenerator> _logger;
        private readonly IRabbitMqClientReceiver _clientReceiver;
        private readonly IConfiguration _configuration;
        private readonly FileWriterDbContext _context;


        private static ConnectionFactory _connectionFactory;

        public OutputFileGenerator(ILogger<OutputFileGenerator> logger
            , IConfiguration configuration
            , FileWriterDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public void GenerateFIle(OutputFileContentDto outputDto)
        {
            _logger.LogInformation($"{outputDto.FileName} output is been generated.");

            var outputContent = new OutputFileContent
            {
                FileName = outputDto.FileName,
                FileExtension = outputDto.FileExtension,
                SalesmenQuantity = outputDto.SalesmenQuantity,
                CustomersQuantity = outputDto.CustomersQuantity,
                MostExpensiveSale = outputDto.MostExpensiveSale,
                WorstSalesman = outputDto.WorstSalesman
            };

            var folderFiles = GetFilesInfolder();

            var fileFullName = AssertDuplicates(folderFiles, outputContent);

            var template = File.ReadAllText(_configuration["TemplateFullPath"]);

            if (string.IsNullOrEmpty(template))
            {
                _logger.LogCritical("Template file cannot be found");
                return;
            }

            var replacedTemplate = ReplaceAllPlaceHolders(template, outputContent);

            _logger.LogInformation("Template replaced, writing output file");

            File.WriteAllText(fileFullName, replacedTemplate);

            _logger.LogInformation("Saving processed file into the database for service trace history");

            _context.Add((object) outputContent);

            _context.SaveChanges();
        }

        private List<FileInfo> GetFilesInfolder()
        {
            return new DirectoryInfo(_configuration["OutputPath"]).GetFiles().ToList();
        }

        private string AssertDuplicates(List<FileInfo> filesInfolder, OutputFileContent file)
        {
            var fullPath = $"{_configuration["OutputPath"]}{file.FileName}";

            return filesInfolder.Any(f => f.Name == file.FileName)
                ? fullPath.Replace(file.FileExtension
                    , $" - {filesInfolder.Count(f => f.Name == file.FileName) + 1}{file.FileExtension}")
                : fullPath;
        }

        private string ReplaceAllPlaceHolders(string template, OutputFileContent file)
        {
            return template
                .Replace(_configuration["FileNamePlaceHolder"], file.FileName)
                .Replace(_configuration["SalesmenQuantityPlaceHolder"], file.SalesmenQuantity.ToString())
                .Replace(_configuration["CustomersQuantityPlaceHolder"], file.CustomersQuantity.ToString())
                .Replace(_configuration["MostExpensiveSalePlaceHolder"], file.MostExpensiveSale.ToString())
                .Replace(_configuration["WorstSalesmanPlaceHolder"], file.WorstSalesman)
                .Replace(_configuration["GeneratedAtPlaceHolder"], file.GenerationDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}