using FN.DataAccess.Enums;

namespace FN.ViewModel.Systems.Events
{
    public class EventCreateOrUpdateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SeasonType Season { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class EventProductRequest
    {
        public int EventId { get; set; }
        public int ProductId { get; set; }
        public decimal DiscountPrice { get; set; }
        public int MaxPurchases { get; set; }
    }
    public class AddProductsToEventRequest
    {
        public List<int> ProductIds { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int MaxPurchasesPerProduct { get; set; } = 100;
    }
}
