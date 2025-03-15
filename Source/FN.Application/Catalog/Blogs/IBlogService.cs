using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;

namespace FN.Application.Catalog.Blogs
{
    public interface IBlogService
    {
        Task<ApiResult<int>> CreateItem(BaseCreateRequest request, int userId);
        Task<ApiResult<int>> CreateBlog(BlogCreateRequest request, int itemId);
    }
}
