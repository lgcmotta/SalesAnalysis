using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.DTO;
using SalesAnalysis.SalesProcessor.Core.Domain;
using SalesAnalysis.SalesProcessor.Core.Interfaces;

namespace SalesAnalysis.SalesProcessor.Application.BusinessLogic
{
    public class SalesFileAnalyzer : ISalesFileAnalyser
    {
        private readonly ILogger<SalesFileAnalyzer> _logger;
        private readonly IConfiguration _configuration;
        private readonly ISalesDataProcessor _salesDataProcessor;
        private readonly IRabbitMqClientPublisher _publisher;

        public SalesFileAnalyzer(ILogger<SalesFileAnalyzer> logger, IConfiguration configuration, ISalesDataProcessor salesDataProcessor, IRabbitMqClientPublisher publisher)
        {
            _logger = logger;
            _configuration = configuration;
            _salesDataProcessor = salesDataProcessor;
            _publisher = publisher;
        }

        public async Task ProcessInputFile(InputFile inputFile)
        {
            try
            {
                var fullPath = string.Concat(inputFile.FilePath, "/", inputFile.FileName);

                if (VerifyFileExistence(inputFile, fullPath))
                    return;

                var fileContent = await File.ReadAllLinesAsync(fullPath, CancellationToken.None);

                if (fileContent == null || fileContent.Length == 0)
                {
                    await ProcessFileFailed(inputFile);
                    return;
                }
                
                var contentDto = new FileContentDto {InputFile = inputFile};

                contentDto.InputFile.Processed = true;
                contentDto.InputFile.Canceled = false;
                contentDto.InputFile.ProcessDate = DateTime.Now;
                
                foreach (var line in fileContent)
                {
                    if (line.StartsWith(_configuration["SalesmanIdentifier"]))
                        AddSalesman(line, contentDto);
                    if (line.StartsWith(_configuration["CustomerIdentifier"]))
                        AddCustomer(line, contentDto);
                    if(line.StartsWith(_configuration["SaleIdentifier"]))
                        AddSale(line, contentDto);
                }

                if (!contentDto.Customers.Any()
                    && !contentDto.Sales.Any()
                    && !contentDto.Salesmen.Any())
                {
                    await ProcessFileFailed(inputFile);
                    return;
                }

                await _salesDataProcessor.SaveContentToDatabase(contentDto);

            }
            catch (Exception exception)
            {
                _logger.LogCritical("An unexpected exception occurred when processing input file {FileName}", inputFile.FileName);
                _logger.LogCritical("Exception {message}",exception.Message);
                _logger.LogTrace(exception.StackTrace);
            }
        }

        private async Task ProcessFileFailed(InputFile inputFile)
        {
            _logger.LogCritical("{FileName} couldn't be processed.", inputFile.FileName);

            await _publisher.PublishAsync(inputFile
                , _configuration["RabbitMqHostnName"]
                , _configuration["RabbitMqUserName"]
                , _configuration["RabbitMqPassword"]
                , int.Parse(_configuration["RabbitMqRetryCount"])
                , _configuration["RabbitMqFailedQueueName"]);

            return;
        }

        private void AddCustomer(string line, FileContentDto viewModel)
        {
            var customer = GetStandardLine(line);
            if (customer == null)
                return;
            viewModel.Customers.Add(new Customer
            {
                Cnpj = customer.Value.first,
                Name = customer.Value.second,
                BusinessArea = customer.Value.third.ToString()
            });
        }

        private void AddSalesman(string line, FileContentDto viewModel)
        {
            var salesman = GetStandardLine(line);
            if (salesman == null)
                return;

            viewModel.Salesmen.Add(new Salesman
            {
                Cpf = salesman.Value.first,
                Name = salesman.Value.second,
                Salary = float.Parse(salesman.Value.third.ToString())
            });
        }

        private void AddSale(string line, FileContentDto viewModel)
        {
            var lineContent = GetStandardLine(line);

            if (lineContent == null)
                return;

            var salesData = lineContent.Value.second.Split(_configuration["SalesSeparator"]);

            if (salesData.Length < 0)
                return;

            var saleInfo = (from saleData in salesData
                select saleData.Split(_configuration["SaleDataSeparator"]) 
                into data where data.Length >= 0
                select (int.Parse(data[0].Replace(_configuration["SaleDataStartDelimiter"],string.Empty))
                    , int.Parse(data[1])
                    , float.Parse(data[2].Replace(_configuration["SaleDataEndDelimiter"], string.Empty))))
                .ToList();

            var sale = new Sale
            {
                 SaleId = int.Parse(lineContent.Value.first)
                , SalesmanName = lineContent.Value.third.ToString()
                , SalesInfo = new List<SaleInfo>()
            };

            saleInfo.ForEach(s => sale.SalesInfo.Add(new SaleInfo
            {
                ItemId = s.Item1,
                ItemQuantity = s.Item2,
                ItemPrice = s.Item3
            }));
            

            viewModel.Sales.Add(sale);

        }

        private (string first, string second, object third)? GetStandardLine(string line)
        {
            var content = line.Split(_configuration["ColumnSeparator"]);
            if (content.Length < 1 || content.Length > 4)
                return null;

            return (content[1], content[2], content[3]);
        }

        private bool VerifyFileExistence(InputFile inputFile, string fullPath)
        {
            if (File.Exists(fullPath))
                return false;
            _logger.LogError("File not found: {FileName}", inputFile.FileName);
            return true;
        }
    }
}