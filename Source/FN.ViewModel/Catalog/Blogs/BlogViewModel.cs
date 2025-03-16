using FN.ViewModel.Helper.Paging;

namespace FN.ViewModel.Catalog.Blogs
{
    public class BlogPagingReques : PagedList
    {
        public string? KeyWord { get; set; }
    }
    public class BlogViewModel
    {
        public int Id { get; set; }
        public int BlogId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime TimeCreate { get; set; }
        public string Author { get; set; }
        public int ViewCount { get; set; }
        public string Thumbnail { get; set; }
        public string SeoAlias { get; set; }
    }
    public class BlogDetailViewModel : BlogViewModel
    {
        public string SeoTitle { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string Detail { get; set; }
        public string Username { get; set; }
        public DateTime TimeUpdate { get; set; }
    }
}
