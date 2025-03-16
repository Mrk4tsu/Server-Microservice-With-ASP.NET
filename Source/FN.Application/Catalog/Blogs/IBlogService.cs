using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;

namespace FN.Application.Catalog.Blogs
{
    public interface IBlogService
    {
        Task<ApiResult<int>> CreateCombine(BlogCombineCreateOrUpdateRequest request, int userId);
        Task<ApiResult<int>> UpdateCombine(BlogCombineCreateOrUpdateRequest request, int itemId, int blogId, int userId); 
        Task<ApiResult<PagedResult<BlogViewModel>>> GetBlogs(BlogPagingReques request);
        Task<ApiResult<BlogDetailViewModel>> GetDetail(int id);
    }
}
