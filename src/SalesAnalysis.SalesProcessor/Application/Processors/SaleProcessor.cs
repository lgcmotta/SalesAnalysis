using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SalesAnalysis.SalesProcessor.Core.Domain;
using SalesAnalysis.SalesProcessor.Core.Processors;
using SalesAnalysis.SalesProcessor.Core.ViewModel;

namespace SalesAnalysis.SalesProcessor.Application.Processors
{
    public class SaleProcessor : ISalesProcessor
    {
        private readonly ILogger<SaleProcessor> _logger;
        private readonly IConfiguration _configuration;

        public SaleProcessor(ILogger<SaleProcessor> logger, IConfiguration configuration)
        {
            _logger = logger;
            
            _configuration = configuration;
        }

        public async Task ProcessInputFile(InputFile inputFile)
        {
            try
            {
                var fullPath = string.Concat(inputFile.FilePath, "/", inputFile.FileName);

                if (VerifyFileExistence(inputFile, fullPath))
                    return;

                var fileContent = await File.ReadAllLinesAsync(fullPath, CancellationToken.None);

                var viewModel = new FileContentViewModel();

                foreach (var line in fileContent)
                {
                    if (line.StartsWith(_configuration["SalesmanIdentifier"]))
                        AddSalesman(line, viewModel);
                    if (line.StartsWith(_configuration["CustomerIdentifier"]))
                        AddCustomer(line, viewModel);
                    if(line.StartsWith(_configuration["SaleIdentifier"]))
                        AddSale(line, viewModel);
                }

            }
            catch (Exception exception)
            {
                
            }
        }

        private void AddCustomer(string line, FileContentViewModel viewModel)
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

        private void AddSalesman(string line, FileContentViewModel viewModel)
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

        private void AddSale(string line, FileContentViewModel viewModel)
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
                select (int.Parse(data[0]), int.Parse(data[1]), float.Parse(data[2])))
                .ToList();

            var sale = new Sale
            {
                InputFile = viewModel.InputFile
                , SaleId = int.Parse(lineContent.Value.first)
                , SalesmanName = lineContent.Value.third.ToString()
                , SaleInfo = new List<SaleInfo>()
            };

            saleInfo.ForEach(s => sale.SaleInfo.Add(new SaleInfo
            {
                ItemId = s.Item1,
                Quantity = s.Item2,
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