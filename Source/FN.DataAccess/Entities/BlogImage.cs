namespace FN.DataAccess.Entities
{
    public class BlogImage
    {
        public int Id { get; set; }
        public string PublicId { get; set; }
        public string Caption { get; set; }
        public int BlogId { get; set; }
        public string ImageUrl { get; set; }
        public Blog Blog { get; set; }
    }
}
