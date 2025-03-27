namespace FN.ViewModel.Systems.Order
{
    public class OrderCreateRequest
    {
        public int ProductId { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountPrice { get; set; }
    }
}
