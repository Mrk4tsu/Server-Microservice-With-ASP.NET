﻿using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;

namespace FN.Application.Catalog.Product
{
    public interface IProductManageService
    {
        Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request, int userId);
        Task<ApiResult<PagedResult<ProductViewModel>>> TrashProducts(ProductPagingRequest request, int userId);
        Task<ApiResult<int>> Create(CombinedCreateOrUpdateRequest request, int userId);
        Task<ApiResult<bool>> Update(CombinedCreateOrUpdateRequest request, int itemId, int productId, int userId);
        Task<ApiResult<bool>> DeletePermanently(int itemId, int userId);
        Task<ApiResult<bool>> Delete(int itemId, int userId);
        Task<ApiResult<bool>> DeleteImage(DeleteProductImagesRequest request);
        Task RemoveCacheData();
    }
}
