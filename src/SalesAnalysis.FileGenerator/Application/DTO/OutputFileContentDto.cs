using System;

namespace SalesAnalysis.FileGenerator.Application.DTO
{
    public class OutputFileContentDto
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