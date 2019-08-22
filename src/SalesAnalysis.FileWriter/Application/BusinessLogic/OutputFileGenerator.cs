using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalysis.FileWriter.Application.DTO;
using SalesAnalysis.FileWriter.Core.Domain;
using SalesAnalysis.FileWriter.Core.Interfaces;
using SalesAnalysis.FileWriter.Infrastructure.Persistence;
using SalesAnalysis.RabbitMQ.Interfaces;

namespace SalesAnalysis.FileWriter.Application.BusinessLogic
{
    public class OutputFileGenerator : IOutputFileGenerator
    {
        private readonly ILogger<OutputFileGenerator> _logger;
        private readonly IRabbitMqClientReceiver _clientReceiver;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly FileWriterDbContext _context;

        public OutputFileGenerator(ILogger<OutputFileGenerator> logger
            , IConfiguration configuration
            , IMapper mapper
            , FileWriterDbContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _context = context;
        }

        public void GenerateFIle(OutputFileContentDto outputDto)
        {
            _logger.LogInformation($"{outputDto.FileName} output is been generated.");

            var outputContent = _mapper.Map<OutputFileContent>(outputDto);

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

            _context.Add(outputContent);

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