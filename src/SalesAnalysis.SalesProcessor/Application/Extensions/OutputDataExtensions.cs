using System;
using System.Collections.Generic;
using System.Linq;
using SalesAnalysis.SalesProcessor.Application.DTO;
using SalesAnalysis.SalesProcessor.Infrastructure.Persistence;

namespace SalesAnalysis.SalesProcessor.Application.Extensions
{
    public static class OutputDataExtensions
    {
        public static OutputDataDto GetCustomersQuantity(this OutputDataDto outputDto, FileContentDto contentDto)
        {
            outputDto.CustomersQuantity = contentDto.Customers.DistinctBy(c => c.Cnpj).Count();

            return outputDto;
        }

        public static OutputDataDto GetSalesmenQuantity(this OutputDataDto outputDto, FileContentDto contentDto)
        {
            outputDto.SalesmenQuantity = contentDto.Salesmen.DistinctBy(s => s.Cpf).Count();

            return outputDto;
        }

        public static OutputDataDto GetIdFromMostExpensiveSale(this OutputDataDto outputDto, FileContentDto contentDto
            , SalesProcessorDbContext context)
        {
            var sales = context.Sales.Where(s => s.InputFileName == contentDto.InputFile.FileName).ToList();

            var results = new List<Tuple<int, float>>();

            sales.ForEach(s =>
            {
                var price = s.SalesInfo.Sum(saleInfo => saleInfo.ItemPrice);
                results.Add(Tuple.Create(s.SaleId, price));
            });

            results = results.OrderByDescending(x => x.Item2).ToList();

            outputDto.MostExpensiveSale = results.First().Item1;

            return outputDto;
        }

        public static OutputDataDto GetWorstSalesman(this OutputDataDto outputDto, FileContentDto contentDto)
        {
            var worstSalesman = contentDto.Sales.DistinctBy(x => x.SalesmanName).Min(x => x.SalesmanName);

            outputDto.WorstSalesman = worstSalesman;

            return outputDto;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            var hashSet = new HashSet<TKey>();
            foreach (var element in source)
                if (hashSet.Add(selector(element)))
                    yield return element;
        }
    }
}