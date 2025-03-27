using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class Payment
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string TransactionId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PaymentFee { get; set; }
        public UserOrder Order { get; set; }
        public ProductDetail Product { get; set; }
        public AppUser User { get; set; }
    }
}
