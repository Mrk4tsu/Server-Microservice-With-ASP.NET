using FN.AIService.Models;
using FN.AIService.Services.Chats;
using GeminiAIDev.Models.ContentResponse;
using GeminiAIDev.Models;
using System.Text;
using Newtonsoft.Json;

namespace FN.AIService.Services
{
    public class GeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IChatSessionRepository _chatSessionRepository;
        public GeminiService(HttpClient httpClient, IConfiguration configuration, IChatSessionRepository chatSessionRepository)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GeminiAPIKey"]!;
            _chatSessionRepository = chatSessionRepository;
        }
        public async Task<string> GenerateContentAsync(int userId, string chatSessionId, string userMessage)
        {
            // Lấy lịch sử tin nhắn dưới dạng LinkedList
            var chatMessages = await _chatSessionRepository.GetMessagesAsLinkedListAsync(chatSessionId, userId, 40);

            // Chuyển LinkedList thành danh sách contents cho Gemini API
            var contents = chatMessages.Select(m => new GeminiAIDev.Models.Content
            {
                role = m.Role,
                parts = new[]
                {
                    new GeminiAIDev.Models.Part
                    {
                        text = m.Content
                    }
                }
            }).ToList();

            // Thêm tin nhắn mới của người dùng
            contents.Add(new GeminiAIDev.Models.Content
            {
                role = "user",
                parts = new[] { new GeminiAIDev.Models.Part { text = userMessage } }
            });

            // Kiểm tra tổng độ dài (ước tính token)
            int estimatedTokenCount = contents.Sum(c => c.parts.Sum(p => p.text.Length / 4)); // Ước tính: 1 token ~ 4 ký tự
            if (estimatedTokenCount > 8000) // Giới hạn an toàn
            {
                // Cắt bớt tin nhắn cũ nếu vượt quá
                contents = contents.TakeLast(40).ToList();
            }

            // Gọi API Gemini
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro-exp-03-25:generateContent?key={_apiKey}";
            var request = new ContentRequest
            {
                contents = contents.ToArray()
            };

            string jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var geminiResponse = JsonConvert.DeserializeObject<ContentResponse>(jsonResponse);
                var aiResponse = geminiResponse.Candidates[0].Content.Parts[0].Text;

                // Lưu tin nhắn của người dùng và AI vào MongoDB
                var userMessageEntity = new ChatMessage
                {
                    Role = "user",
                    Content = userMessage,
                    SentAt = DateTime.Now
                };
                var aiMessageEntity = new ChatMessage
                {
                    Role = "model",
                    Content = aiResponse,
                    SentAt = DateTime.Now
                };

                await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, userMessageEntity);
                await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, aiMessageEntity);

                return aiResponse;
            }
            else
            {
                throw new Exception("Error communicating with Gemini API.");
            }
        }
        public async Task<string> GenerateContentAsync(string prompt)
        {
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro-exp-03-25:generateContent?key={_apiKey}";
            var request = new ContentRequest
            {
                contents = new[]
                {
                    new GeminiAIDev.Models.Content
                    {
                        parts = new[]
                        {
                            new GeminiAIDev.Models.Part
                            {
                                text = prompt
                            }
                        }
                    }
                }
            };
            string jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                // You can deserialize jsonResponse if needed
                var geminiResponse = JsonConvert.DeserializeObject<ContentResponse>(jsonResponse);
                return geminiResponse.Candidates[0].Content.Parts[0].Text;
            }
            else
            {
                throw new Exception("Error communicating with Gemini API.");
            }
        }
    }
}
