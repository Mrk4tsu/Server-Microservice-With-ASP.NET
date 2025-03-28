using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Blogs
{
    public class BlogCreateOrUpdateRequest
    {
        public string? Detail { get; set; } = string.Empty;
    }
    public class BlogCombineCreateOrUpdateRequest
    {
        //Item
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public IFormFile? Thumbnail { get; set; }

        //Blog
        public string? Detail { get; set; }
    }
}
