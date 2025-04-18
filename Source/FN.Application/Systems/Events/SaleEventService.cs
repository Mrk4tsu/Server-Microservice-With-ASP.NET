using DnsClient.Internal;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Events;
using Google.Api;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FN.Application.Systems.Events
{
    public class SaleEventService : ISaleEventService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redisService;
        private DateTime _now;
        public SaleEventService(AppDbContext db, IRedisService redisService)
        {
            _db = db;
            _redisService = redisService;
            _now = new TimeHelper.Builder()
                .SetTimestamp(DateTime.UtcNow)
                .SetTimeZone("SE Asia Standard Time")
                .SetRemoveTick(true).Build();
        }

        public async Task ActivateEvent(int eventId)
        { // Đảm bảo chỉ có 1 sự kiện active tại 1 thời điểm
            var activeEvents = await _db.SaleEvents
                .Where(e => e.IsActive)
                .ToListAsync();

            foreach (var evt in activeEvents)
            {
                evt.IsActive = false;
            }
            var eventToActivate = await _db.SaleEvents.FindAsync(eventId);

            if (eventToActivate != null)
            {
                eventToActivate.IsActive = true;
                await _db.SaveChangesAsync();

                // Xóa cache current event
                await _redisService.RemoveValue("current_seasonal_event");

                // Cập nhật ProductPrices cho các sản phẩm trong sự kiện
                await UpdateEventProductPrices(eventId);
            }
            await RemoveOldCache().ConfigureAwait(false);
        }
        public async Task DeactivateEvent(int eventId)
        {
            var eventToDeactivate = await _db.SaleEvents.FindAsync(eventId);
            if (eventToDeactivate != null)
            {
                eventToDeactivate.IsActive = false;
                await _db.SaveChangesAsync();

                // Xóa cache current event
                await _redisService.RemoveValue("current_seasonal_event");
                // Khôi phục giá gốc cho sản phẩm
                await RestoreOriginalPrices(eventId);
            }
            await RemoveOldCache().ConfigureAwait(false);
        }

        public async Task<ApiResult<bool>> AddProductToEvent(EventProductRequest request)
        {
            var eventObj = await _db.SaleEvents.FindAsync(request.EventId);
            var product = await _db.ProductDetails.FindAsync(request.ProductId);

            if (eventObj == null || product == null)
                return new ApiErrorResult<bool>("Event or Product not found");

            var eventProduct = new SaleEventProduct
            {
                SaleEventId = request.EventId,
                ProductDetailId = product.Id,
                DiscountedPrice = request.DiscountPrice,
                MaxPurchases = request.MaxPurchases,
                CurrentPurchases = 0,
                IsActive = true
            };
            _db.SaleEventProducts.Add(eventProduct);

            // Tạo hoặc cập nhật ProductPrice với type Event
            var productPriceExisting = await _db.ProductPrices.FirstOrDefaultAsync(p => p.ProductDetailId == product.Id && p.PriceType == PriceType.SALE_EVENT);
            if (productPriceExisting != null)
            {
                productPriceExisting.Price = request.DiscountPrice;
                productPriceExisting.StartDate = eventObj.StartDate;
                productPriceExisting.EndDate = eventObj.EndDate;
                productPriceExisting.SaleEventId = eventObj.Id;
            }
            else
            {
                var productPrice = new ProductPrice
                {
                    ProductDetailId = product.Id,
                    SaleEventId = eventObj.Id,
                    Price = request.DiscountPrice,
                    PriceType = PriceType.SALE_EVENT,
                    StartDate = eventObj.StartDate,
                    EndDate = eventObj.EndDate
                };
                _db.ProductPrices.Add(productPrice);
            }
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return new ApiSuccessResult<bool>(true);
        }

        public async Task<ApiResult<int>> CreateEvent(EventCreateOrUpdateRequest request)
        {
            var newEvent = new SaleEvent
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsActive = false
            };
            _db.SaleEvents.Add(newEvent);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newEvent.Id);
        }

        public async Task<ApiResult<List<EventProductResponse>>> GetActiveEventProducts()
        {
            var query = _db.SaleEventProducts.Include(ep => ep.ProductDetail).ThenInclude(ep => ep.Item)
                .Include(ep => ep.ProductDetail).ThenInclude(ep => ep.ProductPrices)
                .Include(ep => ep.SaleEvent)
                .Where(ep => ep.IsActive &&
                ep.SaleEvent.StartDate <= _now &&
                ep.SaleEvent.EndDate >= _now).AsQueryable();

            var result = await query.Select(ep => new EventProductResponse
            {
                EventId = ep.SaleEventId,
                EventName = ep.SaleEvent.Name,
                ItemId = ep.ProductDetail.Item.Id,
                ProductId = ep.ProductDetailId,
                ProductName = ep.ProductDetail.Item.Title,
                SeoAlias = ep.ProductDetail.Item.SeoAlias,
                OriginalPrice = ep.ProductDetail.ProductPrices
                .FirstOrDefault(pp => pp.PriceType == PriceType.BASE)!.Price,
                DiscountedPrice = ep.DiscountedPrice,
                Thumbnail = ep.ProductDetail.Item.Thumbnail,
                RemainingPurchases = ep.MaxPurchases - ep.CurrentPurchases,
                EventEndDate = ep.SaleEvent.EndDate,
                PercentageDiscount = (int)(100 - (ep.DiscountedPrice * 100 / ep.ProductDetail.ProductPrices
                .FirstOrDefault(pp => pp.PriceType == PriceType.BASE)!.Price))
            }).ToListAsync();
            return new ApiSuccessResult<List<EventProductResponse>>(result);
        }

        public async Task<ProductPrice?> GetCurrentEventPrice(int productId)
        {
            var result = await _db.ProductPrices
                .Include(x => x.SaleEvent)
                .Where(pp => pp.ProductDetailId == productId
                && pp.PriceType == PriceType.SALE_EVENT
                && pp.SaleEvent.IsActive
                && pp.StartDate <= _now
                && pp.EndDate >= _now).OrderByDescending(pp => pp.StartDate).FirstOrDefaultAsync();

            return result;
        }

        public async Task<ApiResult<int>> ProcessEventPurchase(int eventProductId, int userId)
        {
            var eventProduct = await _db.SaleEventProducts
                .Include(ep => ep.SaleEvent)
                .Include(ep => ep.ProductDetail)
                .FirstOrDefaultAsync(ep => ep.Id == eventProductId);
            if (eventProduct == null) return new ApiErrorResult<int>("Event product not found");
            if (_now < eventProduct.SaleEvent.StartDate || _now > eventProduct.SaleEvent.EndDate)
                return new ApiErrorResult<int>("Event is not active");
            if (eventProduct.CurrentPurchases >= eventProduct.MaxPurchases)
                return new ApiErrorResult<int>("Max purchases reached");
            eventProduct.CurrentPurchases++;

            if (eventProduct.CurrentPurchases >= eventProduct.MaxPurchases)
            {
                eventProduct.IsActive = false;
                var productPrice = await _db.ProductPrices
                .FirstOrDefaultAsync(pp => pp.ProductDetailId == eventProduct.ProductDetailId
                                        && pp.PriceType == PriceType.SALE_EVENT
                                        && pp.SaleEventId == eventProduct.SaleEventId);
                if (productPrice != null)
                    productPrice.EndDate = _now;
            }
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(eventProduct.CurrentPurchases);
        }
        private async Task UpdateEventProductPrices(int eventId)
        {
            var eventProducts = await _db.SaleEventProducts
                   .Include(ep => ep.ProductDetail)
                   .ThenInclude(pd => pd.ProductPrices)
                   .Where(ep => ep.SaleEventId == eventId)
                   .ToListAsync();
            foreach (var eventProduct in eventProducts)
            {
                var regularPrice = eventProduct.ProductDetail.ProductPrices.FirstOrDefault(pp => pp.PriceType == PriceType.BASE);
                if (regularPrice != null)
                {
                    var discountedPrice = regularPrice.Price * (1 - (eventProduct.DiscountPercentage / 100m));
                    eventProduct.DiscountedPrice = discountedPrice;

                    // Cập nhật hoặc tạo ProductPrice với type Event
                    var existingEventPrice = eventProduct.ProductDetail.ProductPrices
                        .FirstOrDefault(pp => pp.PriceType == PriceType.SALE_EVENT);

                    if (existingEventPrice != null)
                    {
                        existingEventPrice.Price = discountedPrice;
                        existingEventPrice.StartDate = eventProduct.SaleEvent.StartDate;
                        existingEventPrice.EndDate = eventProduct.SaleEvent.EndDate;
                    }
                    else
                    {
                        eventProduct.ProductDetail.ProductPrices.Add(new ProductPrice
                        {
                            Price = discountedPrice,
                            PriceType = PriceType.SALE_EVENT,
                            StartDate = eventProduct.SaleEvent.StartDate,
                            EndDate = eventProduct.SaleEvent.EndDate
                        });
                    }
                }
            }
            await _db.SaveChangesAsync();
        }
        private async Task RestoreOriginalPrices(int eventId)
        {
            // Đặt lại ProductPrices có type Event về null hoặc xóa
            var eventProducts = await _db.SaleEventProducts
                .Include(ep => ep.ProductDetail)
                .ThenInclude(pd => pd.ProductPrices)
                .Where(ep => ep.SaleEventId == eventId)
                .ToListAsync();

            foreach (var eventProduct in eventProducts)
            {
                var eventPrices = eventProduct.ProductDetail.ProductPrices
                    .Where(pp => pp.PriceType == PriceType.SALE_EVENT)
                    .ToList();

                foreach (var price in eventPrices)
                {
                    price.EndDate = _now; // Đánh dấu kết thúc
                }
            }

            await _db.SaveChangesAsync();
        }
        private async Task RemoveOldCache(string? param = null)
        {
            await _redisService.RemoveValue(SystemConstant.PRODUCT_KEY).ConfigureAwait(false);
            await _redisService.RemoveCache(SystemConstant.PRODUCT_KEY + "_recommend").ConfigureAwait(false);
            await _redisService.RemoveCache(SystemConstant.PRODUCT_KEY + "_new").ConfigureAwait(false);
            await _redisService.RemoveCache(SystemConstant.PRODUCT_KEY + "_feature").ConfigureAwait(false);
        }

        public async Task<ApiResult<bool>> AddProductToEvent(AddProductsToEventRequest request, int eventId)
        {
            if (request.ProductIds == null || !request.ProductIds.Any())
                return new ApiErrorResult<bool>("Product IDs cannot be null or empty");
            if (request.DiscountPercentage <= 0 || request.DiscountPercentage >= 100)
                return new ApiErrorResult<bool>("Invalid discount percentage");

            var seasonalEvent = await _db.SaleEvents.FirstOrDefaultAsync(e => e.Id == eventId);
            if (seasonalEvent == null)
                return new ApiErrorResult<bool>("Event not found");

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                // Lấy danh sách sản phẩm hiện có trong sự kiện
                var existingProductIds = await _db.SaleEventProducts
                    .Where(sep => sep.SaleEventId == eventId)
                    .Select(sep => sep.ProductDetailId)
                    .ToListAsync();
                // Lọc ra các sản phẩm mới cần thêm
                var newProductIds = request.ProductIds
                    .Except(existingProductIds)
                    .ToList();
                // Kiểm tra sản phẩm có tồn tại không
                var existingProductsCount = await _db.ProductDetails
                    .Where(p => newProductIds.Contains(p.Id))
                    .CountAsync();

                if (existingProductsCount != newProductIds.Count)
                    return new ApiErrorResult<bool>("Some products not found");

                // Thêm sản phẩm vào sự kiện
                foreach (var productId in newProductIds)
                {
                    // Lấy giá gốc của sản phẩm
                    var regularPrice = await _db.ProductPrices
                        .FirstOrDefaultAsync(pp => pp.ProductDetailId == productId && pp.PriceType == PriceType.BASE);
                    if (regularPrice == null)
                        continue; //Bỏ qua sản phẩm lỗi

                    var discountedPrice = regularPrice.Price * (1 - (request.DiscountPercentage / 100m));

                    var seasonalEventProduct = new SaleEventProduct
                    {
                        SaleEventId = eventId,
                        ProductDetailId = productId,
                        DiscountedPrice = discountedPrice,
                        MaxPurchases = request.MaxPurchasesPerProduct,
                        CurrentPurchases = 0,
                        IsActive = true
                    };

                    _db.SaleEventProducts.Add(seasonalEventProduct);
                    // Nếu sự kiện đang active, cập nhật ProductPrice ngay lập tức
                    if (seasonalEvent.IsActive)
                    {
                        var existingEventPrice = await _db.ProductPrices
                            .FirstOrDefaultAsync(pp =>
                                pp.ProductDetailId == productId &&
                                pp.PriceType == PriceType.SALE_EVENT &&
                                pp.EndDate > _now);
                        if (existingEventPrice != null)
                        {
                            existingEventPrice.SaleEventId = eventId;
                            existingEventPrice.Price = discountedPrice;
                            existingEventPrice.StartDate = seasonalEvent.StartDate;
                            existingEventPrice.EndDate = seasonalEvent.EndDate;
                        }
                        else
                        {
                            _db.ProductPrices.Add(new ProductPrice
                            {
                                ProductDetailId = productId,
                                Price = discountedPrice,
                                PriceType = PriceType.SALE_EVENT,
                                StartDate = seasonalEvent.StartDate,
                                EndDate = seasonalEvent.EndDate,
                                SaleEventId = eventId
                            });
                        }
                    }
                }
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<bool>("An error occurred while adding products" + ex.Message);
            }
        }
    }
}
