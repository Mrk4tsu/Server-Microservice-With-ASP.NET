namespace FN.ViewModel.Catalog.Products.Manage
{
    public class ProductOwnerViewModel
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SeoAlias { get; set; } = string.Empty;
        public List<string> Url { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
