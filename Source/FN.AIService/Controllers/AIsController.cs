using FN.AIService.Models;
using FN.AIService.Services;
using FN.AIService.Services.Chats;
using GeminiAIDev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FN.AIService.Controllers
{
    [Route("api/ai")]
    [ApiController, Authorize]
    public class AIsController : BasesController
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
        [HttpPost("stream-assistant")]
        public async Task StreamAssistant([FromBody] AssistantRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                Response.StatusCode = 401;
                return;
            }
            await _analysisService.AssistantsChatStream(request, userId.Value, HttpContext);
        }
        [HttpPost("assistant")]
        public async Task<IActionResult> GetAssistant(AssistantRequest request)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                if (userId == null) return Unauthorized();
                var result = await _analysisService.AssistantsChat(request, userId.Value);
                //return CreateAction về Id của session chat
                return CreatedAtAction(nameof(CreateChatSession), new { SessionId = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // Tạo phiên chat mới
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateChatSession()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var session = new ChatSession
            {
                UserId = userId.Value,
                Title = $"Chat {DateTime.Now:yyyy-MM-dd HH:mm:ss}",
                CreatedDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                IsActive = true
            };
            await _chatSessionRepository.CreateChatSessionAsync(session);
            return Ok(new { SessionId = session.Id });
        }
        [HttpPost("stream-message")]
        public async Task StreamMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                Response.StatusCode = 401;
                return;
            }

            var session = await _chatSessionRepository.GetChatSessionAsync(request.ChatSessionId, userId.Value);
            if (session == null)
            {
                Response.StatusCode = 404;
                return;
            }

            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            await foreach (var chunk in _geminiService.StreamGenerateContentAsync(userId.Value, request.ChatSessionId, request.Message))
            {
                // Chỉ gửi nội dung text, không gửi cả object JSON
                await Response.WriteAsync($"data: {JsonConvert.SerializeObject(chunk)}\n\n");
                await Response.Body.FlushAsync();
            }

            await Response.WriteAsync("data: [DONE]\n\n");
            await Response.Body.FlushAsync();
        }
        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var session = await _chatSessionRepository.GetChatSessionAsync(request.ChatSessionId, userId.Value);

            if (session == null)
                return NotFound("Chat session not found or unauthorized.");

            var response = await _geminiService.GenerateContentAsync(userId.Value, request.ChatSessionId, request.Message);

            return Ok(new { Response = response });
        }
        [HttpGet("session/{chatSessionId}/history")]
        public async Task<IActionResult> GetChatHistory(string chatSessionId, int page = 1, int pageSize = 20)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var messages = await _chatSessionRepository.GetMessagesAsLinkedListAsync(chatSessionId, userId.Value);

            var pagedMessages = messages
                .Where(x => x.IsDeleted == false)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new { m.Role, m.Content, m.SentAt })
                .ToList();

            return Ok(pagedMessages);
        }
        [HttpDelete("session/{chatSessionId}")]
        public async Task<IActionResult> DeleteChatSession(string chatSessionId)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            try
            {
                await _chatSessionRepository.DeleteChatSessionAsync(chatSessionId, userId);

                return Ok(new { Message = "Chat session deleted successfully." });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("sessions")]
        public async Task<IActionResult> GetUserChatSessions(int page = 1, int pageSize = 10)
        {

            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var sessions = await _chatSessionRepository.GetUserChatSessionsAsync(userId.Value, page, pageSize);

            return Ok(sessions);
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