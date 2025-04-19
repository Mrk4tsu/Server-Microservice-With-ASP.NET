namespace FN.DataAccess.Entities
{
    public class SaleEventProduct
    {
        public int Id { get; set; }
        public int SaleEventId { get; set; }
        public int ProductDetailId { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int DiscountPercentage { get; set; }
        public int MaxPurchases { get; set; } // Số lượt mua tối đa
        public int CurrentPurchases { get; set; } // Số lượt mua hiện tại
        public bool IsActive { get; set; }

        public SaleEvent SaleEvent { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}
