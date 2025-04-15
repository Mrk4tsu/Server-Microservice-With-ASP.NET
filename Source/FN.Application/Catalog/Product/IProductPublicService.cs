using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.FeedbackProduct;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;

namespace FN.Application.Catalog.Product
{
    public interface IProductPublicService
    {
        Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request);
        Task<ApiResult<ProductDetailViewModel>> GetProduct(int productId, int userId);
        Task<ApiResult<ProductDetailViewModel>> GetProductWithoutLogin(int productId);
        Task<ApiResult<List<ProductViewModel>>> GetProducts(string type, int take);
        Task<ApiResult<int>> AddProductFeedback(FeedbackRequest request, int userId);
        Task<ApiSuccessResult<PagedResult<FeedbackViewModel>>> GetFeedbackProduct(PagedList request, int productId);
    }
}
