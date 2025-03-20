using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog
{
    public class BaseRequest
    {
        public string? Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string? Keywords { get; set; } = string.Empty;
        public IFormFile? Thumbnail { get; set; }
    }
}
