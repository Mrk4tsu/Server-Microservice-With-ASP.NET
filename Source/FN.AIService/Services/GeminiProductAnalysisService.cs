using Newtonsoft.Json;

namespace FN.AIService.Services
{
    public class GeminiProductAnalysisService : IGeminiProductAnalysisService
    {
        private readonly GeminiService _geminiService;
        private readonly IProductService _productService;
        public GeminiProductAnalysisService(GeminiService geminiService, IProductService productService)
        {
            _geminiService = geminiService;
            _productService = productService;
        }
        public async Task<string> GetEnhancedProductAnalysisAsync()
        {
            var statistics = await _productService.GetProductsStatisticsAsync();
            var topProducts = await _productService.GetTopViewedItemsAsync(10);
            var topRated = await _productService.GetTopRatedProductsAsync(10);

            var prompt = $"""
            Bạn là một chuyên gia phân tích sản phẩm. 
            Hãy phân tích dữ liệu sau và đưa ra insights có giá trị:
        
            **Thống kê tổng quan**:
            {statistics}
        
            **Top 10 sản phẩm xem nhiều nhất**:
            {string.Join("\n", topProducts.Select((p, i) => $"{i + 1}. {p.Title} - {p.ViewCount} views"))}
        
            **Top 10 sản phẩm được đánh giá cao nhất**:
            {string.Join("\n", topRated.Select((p, i) => $"{i + 1}. {p.Item.Title} - {p.LikeCount} likes"))}
        
            Yêu cầu:
            1. Phân tích xu hướng sản phẩm phổ biến
            2. Đề xuất 3 chiến lược cải thiện hiệu suất sản phẩm
            3. Dự đoán xu hướng trong thời gian tới
            4. Định dạng kết quả rõ ràng với các tiêu đề phù hợp
            """;

            return await _geminiService.GenerateContentAsync(prompt);
        }
        public async Task<string> GetPersonalizedRecommendationsAsync(int userId)
        {
            // Lấy dữ liệu sản phẩm từ database
            var topProducts = await _productService.GetTopViewedItemsAsync(10);
            var latestProducts = await _productService.GetLatestItemsAsync(10);
            var userProducts = await _productService.GetItemsByUserAsync(userId);

            // Tạo prompt từ dữ liệu
            var prompt = $"Tôi có danh sách sản phẩm với các thông tin sau:\n" +
                         $"Top 10 sản phẩm xem nhiều nhất: {JsonConvert.SerializeObject(topProducts.Select(p => p.Title))}\n" +
                         $"10 sản phẩm mới nhất: {JsonConvert.SerializeObject(latestProducts.Select(p => p.Title))}\n" +
                         $"Sản phẩm của người dùng: {JsonConvert.SerializeObject(userProducts.Select(p => p.Title))}\n\n" +
                         $"Hãy phân tích và đưa ra đề xuất sản phẩm phù hợp cho người dùng này.";

            return await _geminiService.GenerateContentAsync(prompt);
        }

        public async Task<string> GetProductRecommendationsAsync(int userId)
        {
            var userProducts = await _productService.GetItemsByUserAsync(userId);
            var topProducts = await _productService.GetTopViewedItemsAsync(20);

            var prompt = $"Người dùng hiện có các sản phẩm sau:\n" +
                         $"{string.Join("\n", userProducts.Select(p => $"- {p.Title}"))}\n\n" +
                         $"Dựa trên top 20 sản phẩm phổ biến:\n" +
                         $"{string.Join("\n", topProducts.Select(p => $"- {p.Title}"))}\n\n" +
                         $"Hãy đề xuất 5 sản phẩm phù hợp nhất cho người dùng này, kèm theo lý do.";

            return await _geminiService.GenerateContentAsync(prompt);
            
        }

        public async Task<string> GetProductStatisticsAnalysisAsync()
        {
            var statistics = await _productService.GetProductsStatisticsAsync();
            var topRated = await _productService.GetTopRatedProductsAsync(5);

            var prompt = $"Thống kê sản phẩm:\n{statistics}\n\n" +
                         $"Top 5 sản phẩm được đánh giá cao:\n" +
                         $"{string.Join("\n", topRated.Select(p => $"- {p.Item.Title}: {p.LikeCount} lượt thích"))}\n\n" +
                         $"Hãy phân tích xu hướng và đưa ra nhận xét về hiệu suất sản phẩm.";

            return await _geminiService.GenerateContentAsync(prompt);
        }
    }
}
