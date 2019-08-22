using System;

namespace SalesAnalysis.SalesProcessor.Application.DTO
{
    public class OutputDataDto
    {
        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public int CustomersQuantity { get; set; }

        public int SalesmenQuantity { get; set; }

        public int MostExpensiveSale { get; set; }

        public string WorstSalesman { get; set; }

        public DateTime GenerationDate { get; set; }
        
    }
}
