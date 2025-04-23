using FN.AIService.Models;
using FN.AIService.Services;
using FN.AIService.Services.Chats;
using GeminiAIDev.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FN.AIService.Controllers
{
    [Route("api/ai")]
    [ApiController]
    public class AIsController : ControllerBase
    {
        private readonly IGeminiProductAnalysisService _analysisService;
        private readonly IChatSessionRepository _chatSessionRepository;
        private readonly GeminiService _geminiService;
        public AIsController(GeminiService service,
            IChatSessionRepository chatSessionRepository,
            IGeminiProductAnalysisService analysisService)
        {
            _chatSessionRepository = chatSessionRepository;
            _analysisService = analysisService;
            _geminiService = service;
        }
        // Tạo phiên chat mới
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateChatSession()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var session = new ChatSession
            {
                UserId = userId,
                Title = $"Chat {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                IsActive = true
            };
            await _chatSessionRepository.CreateChatSessionAsync(session);
            return Ok(new { SessionId = session.Id });
        }
        // Gửi tin nhắn và nhận phản hồi
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var session = await _chatSessionRepository.GetChatSessionAsync(request.ChatSessionId, userId);

            if (session == null)
                return NotFound("Chat session not found or unauthorized.");

            var response = await _geminiService.GenerateContentAsync(userId, request.ChatSessionId, request.Message);

            return Ok(new { Response = response });
        }
        // Lấy lịch sử trò chuyện với caching
        [HttpGet("session/{chatSessionId}/history")]
        public async Task<IActionResult> GetChatHistory(string chatSessionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var cacheKey = $"ChatHistory_{chatSessionId}";
            // Lấy tin nhắn từ MongoDB dưới dạng LinkedList
            var messages = await _chatSessionRepository.GetMessagesAsLinkedListAsync(chatSessionId, userId);

            if (!messages.Any())
                return NotFound("No messages found.");

            return Ok(messages.Select(m => new { m.Role, m.Content, m.SentAt }));
        }
        [HttpGet("recommendations/{userId}")]
        public async Task<IActionResult> GetRecommendations(int userId)
        {
            try
            {
                var result = await _analysisService.GetProductRecommendationsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo đề xuất: {ex.Message}");
            }
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatisticsAnalysis()
        {
            try
            {
                var result = await _analysisService.GetProductStatisticsAnalysisAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi phân tích thống kê: {ex.Message}");
            }
        }
        [HttpGet("enhanced-product")]
        public async Task<IActionResult> GetEnhancedProductAnalysisAsync()
        {
            try
            {
                var result = await _analysisService.GetEnhancedProductAnalysisAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo phân tích sản phẩm: {ex.Message}");
            }
        }
        [HttpGet("personalized/{userId}")]
        public async Task<IActionResult> GetPersonalizedRecommendations(int userId)
        {
            try
            {
                var result = await _analysisService.GetPersonalizedRecommendationsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo đề xuất cá nhân hóa: {ex.Message}");
            }
        }
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateContent([FromBody] PromptRequest request)
        {
            try
            {
                string content = await _geminiService.GenerateContentAsync(request.Prompt);
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("description")]
        public async Task<IActionResult> GenerateDescription([FromBody] PromptDescriptionRequest request)
        {
            try
            {
                var prompt = $"hãy cho tôi 1 mô tả tổng quát cho sản phẩm lập trình có tên {request.Name} để làm mô tả cho phần SEO với các đặc điểm: Dành cho người dùng {request.Other}, hệ điều hành {request.System}. Hãy vào vấn đề chính, không lan man";
                string content = await _geminiService.GenerateContentAsync(prompt);
                return Ok(content);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
    public class SendMessageRequest
    {
        public string ChatSessionId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}