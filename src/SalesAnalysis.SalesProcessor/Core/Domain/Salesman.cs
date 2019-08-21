using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Core.Domain
{
    public class Salesman : IEntity
    {
        public int Id { get; set; }

        public string Cpf { get; set; }

        public string Name { get; set; }

        public float Salary { get; set; }
        
    }
}