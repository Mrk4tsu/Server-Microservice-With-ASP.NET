namespace FN.DataAccess.Entities
{
    public class FeedBack
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime TimeCreated { get; set; } = DateTime.Now;
        public byte Rate { get; set; }
        public bool Status { get; set; }
        public AppUser User { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}
