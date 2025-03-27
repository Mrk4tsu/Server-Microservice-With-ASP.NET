using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class UserOrder
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public AppUser User { get; set; }
        public ProductDetail Product { get; set; }
        public Payment Payment { get; set; }
    }
}
