using FN.ViewModel.Catalog.ProductItems;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Catalog.Products.Statistical;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.Mvc;

namespace FN.Application.Catalog.Product
{
    public interface IProductManageService
    {
        Task<ApiResult<ManageProductViewModel>> GetProductById(int id);
        Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request, int userId);
        Task<ApiResult<PagedResult<ProductOwnerViewModel>>> GetProductsOwner(ProductPagingRequest request, int userId);
        Task<ApiResult<PagedResult<ProductViewModel>>> TrashProducts(ProductPagingRequest request, int userId);
        Task<ApiResult<int>> Create(CombinedCreateOrUpdateRequest request, int userId);
        Task<ApiResult<bool>> Update(CombinedCreateOrUpdateRequest request, int itemId, int productId, int userId);
        Task<ApiResult<bool>> DeletePermanently(int itemId, int userId);
        Task<ApiResult<bool>> DeleteOrRoleback(int itemId, int userId);
        Task<ApiResult<bool>> DeleteImage(DeleteProductImagesRequest request);
        Task<ApiResult<int>> AddItemProduct(ProductItemRequest request, int productId);
        Task<ApiResult<int>> EditItemProduct(ProductItemSingleRequest request, int itemProductId);
        Task<ApiSuccessResult<List<ProductStatsViewModel>>> GetUserProductsWithStats(int userId);
        Task RemoveCacheData();
    }
}
