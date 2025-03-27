using FN.DataAccess.Enums;
using FN.ViewModel.Helper.Paging;

namespace FN.ViewModel.Systems.Order
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPrice { get; set; }
        public decimal PaymentFee { get; set; }
        public DateTime PaymentDate { get; set; }
    }
    public class PaymentPagingRequest : PagedList
    {
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
