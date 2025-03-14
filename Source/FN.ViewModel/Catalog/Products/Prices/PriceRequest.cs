using FN.DataAccess.Enums;

namespace FN.ViewModel.Catalog.Products.Prices
{
    public class PriceRequest 
    {
        public decimal? Price { get; set; }
        public int ProductId { get; set; }
        public PriceType? PriceType { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
