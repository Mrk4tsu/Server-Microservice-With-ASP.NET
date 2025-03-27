namespace FN.DataAccess.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string PublicId { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public int ProductDetailId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public ProductDetail ProductDetail { get; set; }
    }
}
