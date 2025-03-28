using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;

namespace FN.Application.Catalog.Product
{
    public interface IProductPublicService
    {
        Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request);
        Task<ApiResult<ProductDetailViewModel>> GetProduct(int itemId, int userId);
        Task<ApiResult<ProductDetailViewModel>> GetProductWithoutLogin(int productId);
        Task<ApiResult<PagedResult<ProductViewModel>>> GetProductsOwner(ProductPagingRequest request, int userId);
    }
}
