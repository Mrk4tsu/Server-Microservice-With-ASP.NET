namespace FN.ViewModel.Systems.Events
{
    public class EventProductResponse
    {
        public int EventId { get; set; }
        public string EventName { get; set; }
        public int ItemId { get; set; }
        public int ProductId { get; set; }
        public string SeoAlias { get; set; }
        public string ProductName { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public string Thumbnail { get; set; }
        public int RemainingPurchases { get; set; }
        public DateTime EventEndDate { get; set; }
        public int PercentageDiscount { get; set; }
    }
}
