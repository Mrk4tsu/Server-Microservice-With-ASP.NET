using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Blogs
{

    public class BlogCreateRequest
    {
        public string Detail { get; set; } = string.Empty;
    }
    public class BlogCombineCreateRequest
    {
        //Item
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public IFormFile Thumbnail { get; set; }

        //Blog
        public string Detail { get; set; }
    }
}
