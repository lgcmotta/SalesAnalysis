using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Helpers;
using SalesAnalysis.RabbitMQ.Interfaces;
using SalesAnalysis.SalesProcessor.Application.DTO;
using SalesAnalysis.SalesProcessor.Core.Domain;
using SalesAnalysis.SalesProcessor.Core.Interfaces;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Application.BusinessLogic
{
    public class SalesDataProcessor : ISalesDataProcessor
    {
        private readonly ILogger<SalesDataProcessor> _logger;
        private readonly SalesProcessorDbContext _context;
        private readonly IOutputDataProcessor _processor;

        public SalesDataProcessor(ILogger<SalesDataProcessor> logger, SalesProcessorDbContext context,  IOutputDataProcessor processor)
        {
            _logger = logger;
            _context = context;
            _processor = processor;
        }

        public async Task SaveContentToDatabase(FileContentDto content)
        {

            var policy = PolicyHelper.CreateSqlPolicy(_logger, 5);

            await policy.Execute(async () =>
            {
                await AddOrUpdateFile(content.InputFile);

                await AddOrUpdateSalesman(content.Salesmen);

                await AddOrUpdateCustomer(content.Customers);

                await AddOrUpdateSales(content.Sales, content.InputFile.FileName);

                var saved = await _context.SaveAsync();

                if (saved > 0)
                    _processor.BuildOutputData(content).GetAwaiter();
          
            });

            //_logger.LogError("An unexpected exception occurred while processing view model data.");
            //_logger.LogError("Exceptiom {message}", exception.Message);
            //_logger.LogTrace(exception.StackTrace);
        }

        private async Task AddOrUpdateSales(List<Sale> sales, string inputFileFileName)
        {
            sales.ForEach(async sale =>
            {
                var fromDb = await _context.Sales.SingleOrDefaultAsync(s => s.SaleId == sale.SaleId);

                if (fromDb == null)
                {
                    sale.InputFileName = inputFileFileName;
                    await _context.Sales.AddAsync(sale);
                    await _context.SalesInfo.AddRangeAsync(sale.SalesInfo);

                }
                else
                {
                    fromDb.SalesmanName = sale.SalesmanName;
                    fromDb.SalesInfo = sale.SalesInfo;
                    fromDb.InputFileName = inputFileFileName;
                    _context.Sales.Update(fromDb);
                    _context.SalesInfo.UpdateRange(fromDb.SalesInfo);
                }
            });
        }

        private async Task AddOrUpdateCustomer(List<Customer> customers)
        {
            customers.ForEach(async customer =>
            {
                var fromDb = await _context.Customers.SingleOrDefaultAsync(c => c.Name == customer.Name
                                                                                && c.Cnpj == customer.Cnpj);
                if (fromDb == null)
                    await _context.Customers.AddAsync(customer);
                else
                {
                    fromDb.Name = customer.Name;
                    fromDb.Cnpj = customer.Cnpj;
                    fromDb.BusinessArea = customer.BusinessArea;

                    _context.Customers.Update(fromDb);
                }
            });
        }

        private async Task AddOrUpdateSalesman(List<Salesman> salesmen)
        {
            salesmen.ForEach(async sm =>
            {
                var fromDb = await _context.Salesmen.SingleOrDefaultAsync(s => s.Name == sm.Name
                                                                               && s.Cpf == sm.Cpf);
                if (fromDb == null)
                {
                    await _context.AddAsync(sm);
                    return;
                }

                fromDb.Name = sm.Name;
                fromDb.Cpf = sm.Cpf;
                fromDb.Salary = sm.Salary;

                _context.Salesmen.Update(fromDb);
            });
            
        }

        private async Task AddOrUpdateFile(InputFile inputFile)
        {
            var fromDb = await _context.InputFiles.SingleOrDefaultAsync(f => f.FileName == inputFile.FileName
                                                                           && f.FileExtension == inputFile.FileExtension
                                                                           && f.Processed);
            if (fromDb != null)
            {
                fromDb.Canceled = false;
                _context.InputFiles.Update(fromDb);
                return;
            }

            inputFile.Id = null;

            await _context.AddAsync(inputFile);

        }
    }
}