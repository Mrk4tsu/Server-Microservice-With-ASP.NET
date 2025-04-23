namespace FN.AIService.Services
{
    public interface IGeminiProductAnalysisService
    {
        Task<string> GetProductRecommendationsAsync(int userId);
        Task<string> GetProductStatisticsAnalysisAsync();
        Task<string> GetPersonalizedRecommendationsAsync(int userId);
        Task<string> GetEnhancedProductAnalysisAsync();
    }
}
