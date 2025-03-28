namespace FN.DataAccess.Entities
{
    public class Category
    {
        public byte Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SeoAlias { get; set; } = string.Empty;
        public string SeoTitle { get; set; } = string.Empty;
        public string SeoDescription { get; set; } = string.Empty;
        public string SeoKeyword { get; set; } = string.Empty;
        public string SeoImage { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
        public bool Status { get; set; }
        public List<ProductDetail> ProductDetails { get; set; }
    }
}
