using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Products.Statistical;

namespace FN.Application.Catalog.Statisticals
{
    public interface IProductStatsRepository
    {
        Task IncrementProductViewAsync(int productId);
        Task IncrementBLogViewAsync(int blogId);
        Task<List<Item>> GetItemsByUser(int userId);
        Task<Dictionary<int, ProductStatsSummary>> GetProductsViewStats(IEnumerable<int> productIds);
        Task<Dictionary<int, ProductStatsSummary>> GetBlogsViewStats(IEnumerable<int> blogIds);
        Task<List<ProductStatistical>> GetProductStatsAsync(int productId, int days = 30);
    }
}
