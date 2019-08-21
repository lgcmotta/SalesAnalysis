using System;
using System.Collections.Generic;
using System.Text;

namespace SalesAnalysis.SalesProcessor.Application.DTO
{
    public class OutputDataDto
    {
        public string FileName { get; set; }

        public int CustomersQuantity { get; set; }

        public int SalesmenQuantity { get; set; }

        public int MostExpensiveSale { get; set; }

        public string WorstSalesman { get; set; }

        public DateTime GenerationDate { get; set; }
    }
}
