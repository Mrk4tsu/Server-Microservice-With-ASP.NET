using FN.ViewModel.Catalog.Products.Prices;
using FN.ViewModel.Helper.API;

namespace FN.Application.Catalog.Product.Prices
{
    public interface IPriceProductService
    {
        Task<ApiResult<int>> Create(PriceRequest request);
        Task<ApiResult<bool>> Update(PriceRequest request);
        Task<ApiResult<bool>> Delete(int id);
    }
}
