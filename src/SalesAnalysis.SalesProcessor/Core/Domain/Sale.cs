﻿using System.Collections.Generic;
using SalesAnalysis.UnitOfWork.Abstractions;

namespace SalesAnalysis.SalesProcessor.Core.Domain
{
    public class Sale : IEntity
    {
        public int Id { get; set; }

        public InputFile InputFile { get; set; }
        
        public int SaleId { get; set; }

        public string SalesmanName { get; set; }

        public ICollection<SaleInfo> SaleInfo { get; set; }
    }
}