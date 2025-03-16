using FN.DataAccess.Enums;
using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Products.Manage
{
    public class ProductDetailUpdateRequest
    {
        public string? Detail { get; set; } = string.Empty;
        public string? Version { get; set; } = string.Empty;
        public string? Note { get; set; } = string.Empty;
        public byte CategoryId { get; set; }
        public ProductType Status { get; set; }
        public List<IFormFile>? NewImages { get; set; }
    }
    public class ItemUpdateDTO
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public IFormFile? Thumbnail { get; set; }
    }
}
