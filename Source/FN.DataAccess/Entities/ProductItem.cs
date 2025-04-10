namespace FN.DataAccess.Entities
{
    public class ProductItem
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public ProductDetail ProductDetail { get; set; }
    }
}
