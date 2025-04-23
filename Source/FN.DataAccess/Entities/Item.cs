namespace FN.DataAccess.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Code { get; set; } = string.Empty;        
        public string Title { get; set; } = string.Empty;
        public string NormalizedTitle { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Cover { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string SeoAlias { get; set; } = string.Empty;
        public string SeoTitle { get; set; } = string.Empty;    
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public AppUser User { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
        public List<Blog> Blogs { get; set; } 
    }
}
