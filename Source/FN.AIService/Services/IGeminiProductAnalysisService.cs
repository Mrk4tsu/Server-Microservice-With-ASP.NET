using FN.AIService.Models;

namespace FN.AIService.Services
{
    public interface IGeminiProductAnalysisService
    {
        Task<string> GetProductRecommendationsAsync(int userId);
        Task<string> GetProductStatisticsAnalysisAsync();
        Task<string> GetPersonalizedRecommendationsAsync(int userId);
        Task<string> GetEnhancedProductAnalysisAsync();
        Task<string> AssistantsChat(AssistantRequest request, int userId);
        Task AssistantsChatStream(AssistantRequest request, int userId, HttpContext context);
    }
}
