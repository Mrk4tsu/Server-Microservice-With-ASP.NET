using FN.DataAccess.Enums;

namespace FN.DataAccess.Entities
{
    public class UserBlogInteraction
    {
        public int UserId { get; set; }
        public int BlogId { get; set; }
        public InteractionType Type { get; set; }
        public DateTime InteractionDate { get; set; } = DateTime.UtcNow;

        public AppUser User { get; set; }
        public Blog Blog { get; set; }
    }
}
