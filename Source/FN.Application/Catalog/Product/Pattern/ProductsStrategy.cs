using AutoMapper;
using AutoMapper.QueryableExtensions;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Helper.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FN.Application.Catalog.Product.Pattern
{
    public interface IProductStrategyFactory
    {
        ProductsStrategy GetStrategy(string type);
    }
    public class ProductStrategyFactory : IProductStrategyFactory
    {
        private readonly Dictionary<string, ProductsStrategy> _strategies;
        public ProductStrategyFactory(NewProductsStrategy newProducts,
            RecommendProductsStrategy recommendProducts,
            FeaturedProductsStrategy featuredProducts)
        {
            _strategies = new Dictionary<string, ProductsStrategy>
            {
                { "new", newProducts },
                { "featured", featuredProducts },
                { "recommend", recommendProducts }
            };
        }
        public ProductsStrategy GetStrategy(string type)
        {
            if (_strategies.TryGetValue(type, out var strategy))
            {
                return strategy;
            }
            throw new ArgumentException($"No strategy found for type: {type}");
        }
    }
    public abstract class ProductsStrategy
    {
        protected readonly AppDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IRedisService _dbRedis;
        protected ProductsStrategy(AppDbContext db, IMapper mapper, IRedisService redis)
        {
            _db = db;
            _mapper = mapper;
            _dbRedis = redis;
        }
        public abstract Task<List<ProductViewModel>> GetProducts(int limit);
    }
    public class RecommendProductsStrategy : ProductsStrategy
    {
        public RecommendProductsStrategy(AppDbContext db, IMapper mapper, IRedisService redis) : base(db, mapper, redis)
        {
        }

        public override async Task<List<ProductViewModel>> GetProducts(int limit)
        {
            var cacheKey = SystemConstant.PRODUCT_KEY + "_recommend";
            if (await _dbRedis.KeyExist(cacheKey))
            {
                var cacheData = await _dbRedis.GetCache(cacheKey);
                if (cacheData != null)
                {
                    var cacheResult = JsonSerializer.Deserialize<List<ProductViewModel>>(cacheData);
                    return cacheResult!;
                }
            }
            var query = _db.ProductDetails.OrderByDescending(p => (p.LikeCount + p.DownloadCount + p.Item.ViewCount))
                .Where(x => x.Item.IsDeleted == false).AsQueryable();
            if (limit > 0)
            {
                query = query.Take(limit);
            }
            var result = await query.ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider).ToListAsync();
            await _dbRedis.SetCache(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            });
            return result;
        }
    }
    public class FeaturedProductsStrategy : ProductsStrategy
    {
        public FeaturedProductsStrategy(AppDbContext db, IMapper mapper, IRedisService redis) : base(db, mapper, redis)
        {
        }

        public override async Task<List<ProductViewModel>> GetProducts(int limit)
        {
            var cacheKey = SystemConstant.PRODUCT_KEY + "_feature";
            if (await _dbRedis.KeyExist(cacheKey))
            {
                var cacheData = await _dbRedis.GetCache(cacheKey);
                if (cacheData != null)
                {
                    var cacheResult = JsonSerializer.Deserialize<List<ProductViewModel>>(cacheData);
                    return cacheResult!;
                }
            }
            var query = _db.ProductDetails.OrderByDescending(p => p.LikeCount)
                .Where(x => x.Item.IsDeleted == false).AsQueryable();
            if (limit > 0)
            {
                query = query.Take(limit);
            }
            var result = await query.ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider).ToListAsync();
            await _dbRedis.SetCache(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            });
            return result;
        }
    }
    public class NewProductsStrategy : ProductsStrategy
    {
        public NewProductsStrategy(AppDbContext db, IMapper mapper, IRedisService redis) : base(db, mapper, redis)
        {
        }
        public override async Task<List<ProductViewModel>> GetProducts(int limit)
        {
            var cacheKey = SystemConstant.PRODUCT_KEY + "_new";
            if (await _dbRedis.KeyExist(cacheKey))
            {
                var cacheData = await _dbRedis.GetCache(cacheKey);
                if (cacheData != null)
                {
                    var cacheResult = JsonSerializer.Deserialize<List<ProductViewModel>>(cacheData);
                    return cacheResult!;
                }
            }
            var query = _db.ProductDetails.OrderByDescending(p => p.Item.CreatedDate)
                .Where(x => x.Item.IsDeleted == false).AsQueryable();
            if (limit > 0)
            {
                query = query.Take(limit);
            }
            var result = await query.ProjectTo<ProductViewModel>(_mapper.ConfigurationProvider).ToListAsync();
            await _dbRedis.SetCache(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = DateTime.Now.AddDays(1)
            });
            return result;
        }
    }
    public class ProductContext
    {
        private ProductsStrategy _strategy;
        public void SetStrategy(ProductsStrategy strategy)
        {
            _strategy = strategy;
        }
        public async Task<List<ProductViewModel>> GetProductsSelection(int limit)
        {
            if (_strategy == null)
            {
                throw new InvalidOperationException("Strategy not set");
            }

            return await _strategy.GetProducts(limit);
        }
    }
}
