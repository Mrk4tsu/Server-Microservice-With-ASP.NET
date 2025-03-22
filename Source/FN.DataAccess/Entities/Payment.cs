using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class Payment
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string TransactionId { get; set; }
        public string Description { get; set; }
        public decimal PaymentFee { get; set; }
        public AppUser User { get; set; }
        public Item Product { get; set; }
    }
}
