using Microsoft.AspNetCore.Identity;

namespace FN.DataAccess.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; } = DateTime.Now;
        public List<Item> Items { get; set; }
        public List<ProductOwner> ProductOwners { get; set; }
        public List<UserBlogInteraction> Interactions { get; set; }
        public List<UserOrder> Orders { get; set; }
        public List<Payment> Payments { get; set; }
        public List<FeedBack> FeedBacks { get; set; }
        public List<UserProductInteraction> UserProductInteractions { get; set; }
    }
}
