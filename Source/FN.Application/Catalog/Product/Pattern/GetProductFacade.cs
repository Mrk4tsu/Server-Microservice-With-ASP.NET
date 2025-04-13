using AutoMapper;
using AutoMapper.QueryableExtensions;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FN.Application.Catalog.Product.Pattern
{
    public class GetProductFacade : BaseService
    {
        private readonly IMapper _mapper;
        public GetProductFacade(AppDbContext db, IRedisService dbRedis, IImageService image, IMapper mapper) 
            : base(db, dbRedis, image, SystemConstant.PRODUCT_KEY)
        {
            _mapper = mapper;
        }

        public async Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request, bool isMe, bool isDeleted, int? currentUserId)
        {
            const string cacheKey = SystemConstant.PRODUCT_KEY;
            List<ProductViewModel>? cachedData = null;
            bool useCache = await _dbRedis.KeyExist(cacheKey);

            if (useCache)
            {
                cachedData = await _dbRedis.GetValue<List<ProductViewModel>>(cacheKey);
            }

            PagedResult<ProductViewModel> result;

            if (useCache && cachedData != null && cachedData.Count > 0)
            {
                var filteredData = ApplyMemoryFilters(cachedData, request, isMe, currentUserId);
                result = CreatePagedResult(filteredData, request);
            }
            else
            {
                var query = BuildBaseQuery(isDeleted);
                var filteredQuery = ApplyDatabaseFilters(query, request, isMe, currentUserId);
                result = await ExecuteDatabasePaging(filteredQuery, request);

                await CacheBaseData(query, cacheKey);
            }

            return new ApiSuccessResult<PagedResult<ProductViewModel>>(result);
        }
        private IQueryable<ProductViewModel> BuildBaseQuery(bool isDeleted)
        {
            return _db.ProductDetails
                .AsNoTracking()
                .Where(pd => pd.Item.IsDeleted == isDeleted && pd.Item.IsDeleted == isDeleted)
                .ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider);
        }
        //private IQueryable<ProductViewModel> BuildBaseQuery(bool isDeleted)
        //{
        //    return _db.ProductDetails
        //        .AsNoTracking()
        //        .Where(pd => pd.Item.IsDeleted == isDeleted && pd.IsDeleted == isDeleted)
        //        .Select(pd => new ProductViewModel
        //        {
        //            Id = pd.Item.Id,
        //            ProductId = pd.Id,
        //            UserId = pd.Item.UserId,
        //            Title = pd.Item.Title,
        //            NormalizeTitle = pd.Item.NormalizedTitle,
        //            CategorySeoAlias = pd.Category.SeoAlias,
        //            SeoAlias = pd.Item.SeoAlias,
        //            CategoryIcon = pd.Category.SeoImage,
        //            DownloadCount = pd.DownloadCount,
        //            TimeCreates = pd.Item.CreatedDate,
        //            TimeUpdates = pd.Item.ModifiedDate,
        //            Thumbnail = pd.Item.Thumbnail,
        //            Username = pd.Item.User.FullName,
        //            Version = pd.Version,
        //            Prices = pd.ProductPrices
        //                .Where(pp => !pp.ProductDetail.IsDeleted && pp.EndDate > Now()) 
        //                .Select(pp => new PriceViewModel
        //                {
        //                    Id = pp.Id,
        //                    Price = pp.Price,
        //                    PriceType = pp.PriceType,
        //                    StartDate = pp.StartDate,
        //                    EndDate = pp.EndDate
        //                })
        //                .ToList(),
        //        });
        //}
        private List<ProductViewModel> ApplyMemoryFilters(List<ProductViewModel> data, ProductPagingRequest request, bool isMe, int? currentUserId)
        {
            var query = data.AsQueryable();
            if (isMe && currentUserId.HasValue) query = query.Where(x => x.UserId == currentUserId.Value);
            if (!string.IsNullOrEmpty(request.CategorySeoAlias)) query = query.Where(x => x.CategorySeoAlias == request.CategorySeoAlias);
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                var kw = request.KeyWord.Trim().ToUpperInvariant();
                query = query.Where(x =>
                    x.NormalizeTitle.Contains(kw) ||
                    x.Title.Contains(request.KeyWord, StringComparison.OrdinalIgnoreCase) ||
                    x.Username.Contains(request.KeyWord, StringComparison.OrdinalIgnoreCase));
            }
            return query
               .OrderByDescending(x => x.TimeCreates)
               .ToList();
        }

        private IQueryable<ProductViewModel> ApplyDatabaseFilters(IQueryable<ProductViewModel> query, ProductPagingRequest request, bool isMe, int? currentUserId)
        {
            if (isMe && currentUserId.HasValue)
            {
                query = query.Where(x => x.UserId == currentUserId.Value);
            }
            if (!string.IsNullOrEmpty(request.CategorySeoAlias))
            {
                query = query.Where(x => x.CategorySeoAlias == request.CategorySeoAlias);
            }

            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                var kw = request.KeyWord.Trim().ToUpperInvariant();
                query = query.Where(x =>
                    x.NormalizeTitle.Contains(kw) ||
                    EF.Functions.Like(x.Title, $"%{request.KeyWord}%") ||
                    EF.Functions.Like(x.Username, $"%{request.KeyWord}%"));
            }

            return query;
        }
        private async Task<PagedResult<ProductViewModel>> ExecuteDatabasePaging(IQueryable<ProductViewModel> query, ProductPagingRequest request)
        {
            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.TimeCreates)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PagedResult<ProductViewModel>
            {
                TotalRecords = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = items
            };
        }
        private PagedResult<ProductViewModel> CreatePagedResult(List<ProductViewModel> data, ProductPagingRequest request)
        {
            return new PagedResult<ProductViewModel>
            {
                TotalRecords = data.Count,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList()
            };
        }
        private async Task CacheBaseData(IQueryable<ProductViewModel> query, string cacheKey)
        {
            var cacheData = await query
                .OrderByDescending(x => x.TimeCreates)
                .ToListAsync();

            await _dbRedis.SetValue(cacheKey, cacheData);
        }
    }
}
