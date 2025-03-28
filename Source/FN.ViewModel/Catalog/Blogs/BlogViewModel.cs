using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Blogs
{
    public class BlogPagingRequest : PagedList
    {
        public string? KeyWord { get; set; }
    }
    public class BlogViewModel
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime TimeCreate { get; set; }
        public string Author { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string Thumbnail { get; set; } = string.Empty;
        public string SeoAlias { get; set; } = string.Empty;    
    }
    public class BlogDetailViewModel : BlogViewModel
    {
        public string SeoTitle { get; set; } = string.Empty;    
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string Detail { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime TimeUpdate { get; set; }
    }
    public class BlogCombineCreateOrUpdateViewModel
    {
        //Item
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Keywords { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;

        //Blog
        public string Detail { get; set; } = string.Empty;
    }
}
