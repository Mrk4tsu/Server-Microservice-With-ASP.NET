namespace FN.DataAccess.Entities
{
    public class BlogImage
    {
        public int Id { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public int BlogId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public Blog Blog { get; set; }
    }
}
