using System.Collections.Generic;
using SalesAnalysis.SalesProcessor.Core.Domain;

namespace SalesAnalysis.SalesProcessor.Core.ViewModel
{
    public class FileContentViewModel
    {
        public InputFile InputFile { get; set; }

        public List<Salesman> Salesmen { get; set; }

        public List<Customer> Customers { get; set; }

        public List<Sale> Sales { get; set; }

        public FileContentViewModel()
        {
            InputFile = new InputFile();
            Salesmen = new List<Salesman>();
            Customers = new List<Customer>();
            Sales = new List<Sale>();
        }
    }
}