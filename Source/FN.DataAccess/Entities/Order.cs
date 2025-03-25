using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public AppUser User { get; set; }
        public Item Product { get; set; }
        public Payment Payment { get; set; }
    }
}
