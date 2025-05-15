using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Products.Statistical;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace FN.Application.Catalog.Statisticals
{
    public class ProductStatsRepository : IProductStatsRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductStats> _productStatsCollection;
        private readonly IMongoCollection<ProductStats> _blogStatsCollection;
        private readonly AppDbContext _db;
        public ProductStatsRepository(IMongoDatabase database, AppDbContext db)
        {
            _db = db;
            _database = database;
            _productStatsCollection = _database.GetCollection<ProductStats>("ProductStats");
            _blogStatsCollection = _database.GetCollection<ProductStats>("BlogStats");
            CreateIndexesAsync().Wait();
        }
        public Task<List<ProductStatistical>> GetProductStatsAsync(int productId, int days = 30)
        {
            throw new NotImplementedException();
        }
        public async Task CreateIndexesAsync()
        {

            // Tạo compound index cho ProductId và Date
            var indexKeysDefinition = Builders<ProductStats>.IndexKeys
                .Ascending(p => p.ProductId)
                .Ascending(p => p.Date);

            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<ProductStats>(indexKeysDefinition, indexOptions);

            await _productStatsCollection.Indexes.CreateOneAsync(indexModel);
            await _blogStatsCollection.Indexes.CreateOneAsync(indexModel);
        }
        public async Task IncrementProductViewAsync(int productId)
        {
            var _now = DateTime.UtcNow.Date;
            var filter = Builders<ProductStats>.Filter.And(
                Builders<ProductStats>.Filter.Eq(p => p.ProductId, productId),
                Builders<ProductStats>.Filter.Eq(p => p.Date, _now.Date));
            // Tăng view count nếu đã tồn tại
            var update = Builders<ProductStats>.Update.Inc(p => p.ViewCount, 1);

            // Thực hiện upsert (update nếu tồn tại, insert nếu chưa)
            var options = new UpdateOptions { IsUpsert = true };
            await _productStatsCollection.UpdateOneAsync(filter, update, options);
            // Xóa các bản ghi cũ hơn 30 ngày
            var thirtyDaysAgo = _now.AddDays(-30);
            var oldRecordsFilter = Builders<ProductStats>.Filter.And(
                Builders<ProductStats>.Filter.Eq(p => p.ProductId, productId),
                Builders<ProductStats>.Filter.Lt(p => p.Date, thirtyDaysAgo)
            );

            await _productStatsCollection.DeleteManyAsync(oldRecordsFilter);
        }

        public async Task<Dictionary<int, ProductStatsSummary>> GetProductsViewStats(IEnumerable<int> productIds)
        {
            var today = DateTime.UtcNow.Date;
            var filter = Builders<ProductStats>.Filter.In(x => x.ProductId, productIds);

            // Fix: Use FindAsync instead of directly calling ToListAsync on the filter
            var statsCursor = await _productStatsCollection.FindAsync(
                Builders<ProductStats>.Filter.And(
                    filter,
                    Builders<ProductStats>.Filter.Gte(x => x.Date, today.AddDays(-30))
                )
            );

            // Fix: Call ToListAsync on the cursor, not the filter
            var stats = await statsCursor.ToListAsync();

            return productIds.ToDictionary(
                id => id,
                id => new ProductStatsSummary
                {
                    TodayViews = stats.FirstOrDefault(x => x.ProductId == id && x.Date == today)?.ViewCount ?? 0,
                    Last3DaysViews = stats.Where(x => x.ProductId == id && x.Date >= today.AddDays(-3)).Sum(x => x.ViewCount),
                    Last7DaysViews = stats.Where(x => x.ProductId == id && x.Date >= today.AddDays(-7)).Sum(x => x.ViewCount),
                    Last30DaysViews = stats.Where(x => x.ProductId == id).Sum(x => x.ViewCount)
                });
        }
        public async Task<List<Item>> GetItemsByUser(int userId)
        {
            return await _db.Items
                .Where(x => x.UserId == userId && !x.IsDeleted)
                .ToListAsync();
        }

        public async Task<Dictionary<int, ProductStatsSummary>> GetBlogsViewStats(IEnumerable<int> blogIds)
        {
            var today = DateTime.UtcNow.Date;
            var filter = Builders<ProductStats>.Filter.In(x => x.ProductId, blogIds);

            // Fix: Use FindAsync instead of directly calling ToListAsync on the filter
            var statsCursor = await _blogStatsCollection.FindAsync(
                Builders<ProductStats>.Filter.And(
                    filter,
                    Builders<ProductStats>.Filter.Gte(x => x.Date, today.AddDays(-30))
                )
            );

            // Fix: Call ToListAsync on the cursor, not the filter
            var stats = await statsCursor.ToListAsync();

            return blogIds.ToDictionary(
                id => id,
                id => new ProductStatsSummary
                {
                    TodayViews = stats.FirstOrDefault(x => x.ProductId == id && x.Date == today)?.ViewCount ?? 0,
                    Last3DaysViews = stats.Where(x => x.ProductId == id && x.Date >= today.AddDays(-3)).Sum(x => x.ViewCount),
                    Last7DaysViews = stats.Where(x => x.ProductId == id && x.Date >= today.AddDays(-7)).Sum(x => x.ViewCount),
                    Last30DaysViews = stats.Where(x => x.ProductId == id).Sum(x => x.ViewCount)
                });
        }

        public async Task IncrementBLogViewAsync(int blogId)
        {
            var _now = DateTime.UtcNow.Date;
            var filter = Builders<ProductStats>.Filter.And(
                Builders<ProductStats>.Filter.Eq(p => p.ProductId, blogId),
                Builders<ProductStats>.Filter.Eq(p => p.Date, _now.Date));
            // Tăng view count nếu đã tồn tại
            var update = Builders<ProductStats>.Update.Inc(p => p.ViewCount, 1);

            // Thực hiện upsert (update nếu tồn tại, insert nếu chưa)
            var options = new UpdateOptions { IsUpsert = true };
            await _blogStatsCollection.UpdateOneAsync(filter, update, options);
            // Xóa các bản ghi cũ hơn 30 ngày
            var thirtyDaysAgo = _now.AddDays(-30);
            var oldRecordsFilter = Builders<ProductStats>.Filter.And(
                Builders<ProductStats>.Filter.Eq(p => p.ProductId, blogId),
                Builders<ProductStats>.Filter.Lt(p => p.Date, thirtyDaysAgo)
            );

            await _blogStatsCollection.DeleteManyAsync(oldRecordsFilter);
        }
    }
}
