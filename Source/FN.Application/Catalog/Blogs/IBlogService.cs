using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;

namespace FN.Application.Catalog.Blogs
{
    public interface IBlogService
    {
        Task<ApiResult<int>> CreateCombine(BlogCombineCreateRequest request, int userId);
    }
}
