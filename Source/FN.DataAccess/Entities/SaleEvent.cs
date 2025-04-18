﻿using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class SaleEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BannerImage { get; set; }
        public SeasonType Season { get; set; }
        public int Year { get; set; }
        public bool IsActive { get; set; }
        public List<SaleEventProduct> Products { get; set; }
        public List<ProductPrice> ProductPrices { get; set; }
    }
}
