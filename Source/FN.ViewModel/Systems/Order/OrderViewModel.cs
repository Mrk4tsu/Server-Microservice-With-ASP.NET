using FN.DataAccess.Enums;
using FN.ViewModel.Helper.Paging;

namespace FN.ViewModel.Systems.Order
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class OrderPagingRequest : PagedList
    {
        public string ProductCode { get; set; } = string.Empty;
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
