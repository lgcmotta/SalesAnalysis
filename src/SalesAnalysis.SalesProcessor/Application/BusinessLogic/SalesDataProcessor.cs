using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SalesAnalysis.RabbitMQ.Helpers;
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

        public void SaveContentToDatabase(FileContentDto content)
        {
            try
            {
                AddOrUpdateFile(content.InputFile);

                AddOrUpdateSalesman(content.Salesmen);

                AddOrUpdateCustomer(content.Customers);

                AddOrUpdateSales(content.Sales, content.InputFile.FileName);

                var saved =  _context.SaveChanges();

                if (saved > 0)
                    _processor.BuildOutputData(content);

            }
            catch (Exception exception)
            {
                _logger.LogError("An unexpected exception occurred while processing view model data.");
                _logger.LogError("Exceptiom {message}", exception.Message);
                _logger.LogTrace(exception.StackTrace);
            }
        }

        private void AddOrUpdateSales(List<Sale> sales, string inputFileFileName)
        {
            sales.ForEach(sale =>
            {
                var fromDb =  _context.Sales.SingleOrDefault(s => s.SaleId == sale.SaleId);

                if (fromDb == null)
                {
                    sale.InputFileName = inputFileFileName;
                     _context.Sales.Add(sale);
                     _context.SalesInfo.AddRange(sale.SalesInfo);

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

        private void AddOrUpdateCustomer(List<Customer> customers)
        {
            customers.ForEach(customer =>
            {
                var fromDb =  _context.Customers.SingleOrDefault(c => c.Name == customer.Name
                                                                                && c.Cnpj == customer.Cnpj);
                if (fromDb == null)
                     _context.Customers.Add(customer);
                else
                {
                    fromDb.Name = customer.Name;
                    fromDb.Cnpj = customer.Cnpj;
                    fromDb.BusinessArea = customer.BusinessArea;

                    _context.Customers.Update(fromDb);
                }
            });
        }

        private void AddOrUpdateSalesman(List<Salesman> salesmen)
        {
            salesmen.ForEach(async sm =>
            {
                var fromDb = _context.Salesmen.SingleOrDefault(s => s.Name == sm.Name
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

        private void AddOrUpdateFile(InputFile inputFile)
        {
            var fromDb = _context.InputFiles.SingleOrDefault(f => f.FileName == inputFile.FileName
                                                                           && f.FileExtension == inputFile.FileExtension
                                                                           && f.Processed);
            if (fromDb != null)
            {
                fromDb.Canceled = false;
                _context.InputFiles.Update(fromDb);
                return;
            }

            inputFile.Id = null;

            _context.Add(inputFile);

        }
    }
}