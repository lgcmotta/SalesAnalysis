using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Core.Domain
{
    public class SaleInfo : IEntity
    {
        public int Id { get; set; }

        public int ItemId { get; set; }

        public int ItemQuantity { get; set; }

        public float ItemPrice { get; set; }

        public int FkSale { get; set; }

        public Sale Sale { get; set; }
    }
}