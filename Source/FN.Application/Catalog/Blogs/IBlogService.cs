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
        Task<ApiResult<PagedResult<BlogViewModel>>> GetBlogs(BlogPagingRequest request);
        Task<ApiResult<PagedResult<BlogViewModel>>> GetMyBlogs(BlogPagingRequest request, int userId);
        Task<ApiResult<BlogCombineCreateOrUpdateViewModel>> GetDetailManage(int id);
        Task<ApiResult<BlogDetailViewModel>> GetDetail(int id, int userId);
        Task<ApiResult<BlogDetailViewModel>> GetDetailWithoutLogin(int id);
        Task<ApiResult<List<BlogViewModel>>> GetLatestBlogs();
        Task<ApiResult<bool>> Delete(int itemId, int userId);
        Task<ApiResult<bool>> DeletePermanently(int itemId, int userId);
        Task<ApiResult<bool>> UpdateView(int itemId);
    }
}
