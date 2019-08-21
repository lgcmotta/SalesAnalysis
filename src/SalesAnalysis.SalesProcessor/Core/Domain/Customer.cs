using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Core.Domain
{
    public class Customer : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Cnpj { get; set; }

        public string BusinessArea { get; set; }

    }
}