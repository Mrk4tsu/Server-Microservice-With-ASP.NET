namespace FN.DataAccess.Entities
{
    public class ProductOwner
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public AppUser User { get; set; }
        public Item Item { get; set; }
    }
}
