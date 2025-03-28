using FN.DataAccess.Enums;
using System.Text.Json.Serialization;

namespace FN.ViewModel.Catalog.Products
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string NormalizeTitle { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string SeoAlias { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public int DownloadCount { get; set; }
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategorySeoAlias { get; set; } = string.Empty;
        public bool IsOwned { get; set; }
        public DateTime TimeCreates { get; set; }
        public DateTime TimeUpdates { get; set; }
        public List<PriceViewModel> Prices { get; set; } = new();
    }
    public class ProductDetailViewModel : ProductViewModel
    {
        public int ProductId { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int DisLikeCount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public List<ImageProductViewModel> Images { get; set; } = new();
    }
    public class PriceViewModel
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public PriceType PriceType { get; set; } // Thêm using FN.DataAccess.Enums;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class ImageProductViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
    }
}
