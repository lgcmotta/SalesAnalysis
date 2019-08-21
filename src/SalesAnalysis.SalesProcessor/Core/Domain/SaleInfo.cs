﻿using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Core.Domain
{
    public class SaleInfo : IEntity
    {
        public int Id { get; set; }

        public int SaleId { get; set; }

        public int ItemId { get; set; }

        public int Quantity { get; set; }

        public float ItemPrice { get; set; }
    }
}