using FN.DataAccess.Entities;

namespace FN.AIService.Services
{
    public interface IProductService
    {
        Task<List<Item>> GetTopViewedItemsAsync(int count);
        Task<List<Item>> GetLatestItemsAsync(int count);
        Task<List<Item>> GetItemsByUserAsync(int userId);
        Task<List<ProductDetail>> GetTopRatedProductsAsync(int count);
        Task<string> GetProductsStatisticsAsync();
    }
}
