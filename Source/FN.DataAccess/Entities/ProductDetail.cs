﻿using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class ProductDetail
    {
        public int Id { get; set; }
        public string Detail { get; set; } = string.Empty;
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public int DownloadCount { get; set; }
        public string Version { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int ItemId { get; set; }
        public byte CategoryId { get; set; }
        public ProductType Status { get; set; }
        public Item Item { get; set; }
        public Category Category { get; set; }
        public List<ProductPrice> ProductPrices { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public List<Payment> Payments { get; set; }
        public List<UserOrder> Orders { get; set; }
        public List<ProductOwner> ProductOwners { get; set; }
        public List<ProductItem> ProductItems { get; set; }
        public List<FeedBack> FeedBacks { get; set; }
        public List<UserProductInteraction> UserProductInteractions { get; set; }
        public List<SaleEventProduct> SaleEventProducts { get; set; }
    }
}
