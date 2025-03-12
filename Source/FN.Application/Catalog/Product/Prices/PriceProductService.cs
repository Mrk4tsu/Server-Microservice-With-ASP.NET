using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Products.Prices;
using FN.ViewModel.Helper.API;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Product.Prices
{
    public class PriceProductService : IPriceProductService
    {
        private readonly AppDbContext _db;
        private readonly IProductManageService _productManageService;
        public PriceProductService(AppDbContext db, IProductManageService productManageService)
        {
            _db = db;
            _productManageService = productManageService;
        }
        public async Task<ApiResult<int>> Create(PriceRequest request)
        {
            var product = await _db.ProductDetails.FindAsync(request.ProductId);
            if (product == null) return new ApiErrorResult<int>("Product not found");
            var newPrice = new ProductPrice()
            {
                Price = request.Price!.Value,
                ProductDetailId = request.ProductId,
                PriceType = request.PriceType!.Value,
                StartDate = request.FromDate!.Value,
                EndDate = request.ToDate!.Value,
                CreatedDate = DateTime.Now
            };
            _db.ProductPrices.Add(newPrice);

            await _db.SaveChangesAsync();
            await _productManageService.RemoveCacheData();
            return new ApiSuccessResult<int>(newPrice.Id);
        }

        public async Task<ApiResult<bool>> Delete(int id)
        {
            var price = await _db.ProductPrices.FindAsync(id);
            if(price == null) return new ApiErrorResult<bool>("Price not found");
            _db.ProductPrices.Remove(price);
            await _db.SaveChangesAsync();
            await _productManageService.RemoveCacheData();
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<bool>> Update(PriceRequest request)
        {
            var price = await _db.ProductPrices
                .Include(x => x.ProductDetail)
                .FirstOrDefaultAsync(x => x.ProductDetailId == request.ProductId);
            if (price == null) return new ApiErrorResult<bool>("Price not found");
            if (request.Price != null)
                price.Price = request.Price.Value;

            _db.ProductPrices.Update(price);
            await _db.SaveChangesAsync();

            await _productManageService.RemoveCacheData();
            return new ApiSuccessResult<bool>();
        }
    }
}
