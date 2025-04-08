using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class UserProductInteraction
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public InteractionType Type { get; set; }
        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;
        public ProductDetail ProductDetail { get; set; }
        public AppUser User { get; set; }
    }
}
