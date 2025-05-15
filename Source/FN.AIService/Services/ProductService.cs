using FN.DataAccess;
using FN.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace FN.AIService.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        public ProductService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<List<Item>> GetItemsByUserAsync(int userId)
        {
            return await _db.Items
                .AsNoTracking()
                .Where(i => !i.IsDeleted && i.UserId == userId)
                .Include(i => i.ProductDetails)
                .ToListAsync();
        }

        public async Task<List<Item>> GetLatestItemsAsync(int count)
        {
            return await _db.Items.AsNoTracking()
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.CreatedDate)
            .Take(count)
            .Include(i => i.ProductDetails)
            .ToListAsync();
        }

        public async Task<string> GetProductsStatisticsAsync()
        {
            var totalProducts = await _db.Items.CountAsync(i => !i.IsDeleted);
            var totalViews = await _db.Items.SumAsync(i => i.ViewCount);
            var avgViews = totalViews / Math.Max(1, totalProducts);
            var topCategories = await _db.ProductDetails
                .GroupBy(pd => pd.CategoryId)
                .Select(g => new { CategoryId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(3)
                .ToListAsync();

            return $"Tổng số sản phẩm: {totalProducts}, Tổng lượt xem: {totalViews}, " +
                   $"Trung bình lượt xem/sản phẩm: {avgViews}, " +
                   $"Top 3 danh mục: {string.Join(", ", topCategories.Select(x => $"Danh mục {x.CategoryId}: {x.Count} sản phẩm"))}";
        }

        public async Task<List<ProductDetail>> GetTopRatedProductsAsync(int count)
        {
            return await _db.ProductDetails
            .OrderByDescending(pd => pd.LikeCount)
            .Take(count)
            .Include(pd => pd.Item)
            .ThenInclude(i => i.User)
            .ToListAsync();
        }

        public async Task<List<Item>> GetTopViewedItemsAsync(int count)
        {
            return await _db.Items
            .Where(i => !i.IsDeleted)
            .OrderByDescending(i => i.ViewCount)
            .Take(count)
            .Include(i => i.ProductDetails)
            .ThenInclude(pd => pd.ProductImages)
            .ToListAsync();
        }
    }
}
