using System.Collections.Generic;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Application.DTO
{
    public class FileContentDto
    {
        public InputFile InputFile { get; set; }

        public List<Salesman> Salesmen { get; set; }

        public List<Customer> Customers { get; set; }

        public List<Sale> Sales { get; set; }

        public FileContentDto()
        {
            InputFile = new InputFile();
            Salesmen = new List<Salesman>();
            Customers = new List<Customer>();
            Sales = new List<Sale>();
        }
    }
}