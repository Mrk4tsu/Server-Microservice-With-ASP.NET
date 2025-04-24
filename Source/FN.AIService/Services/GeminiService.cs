using FN.AIService.Models;
using FN.AIService.Services.Chats;
using GeminiAIDev.Models.ContentResponse;
using GeminiAIDev.Models;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

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
        //public async IAsyncEnumerable<string> GenerateContentStreamAsync(int userId, string chatSessionId, string userMessage)
        //{
        //    // Lấy lịch sử tin nhắn
        //    var chatMessages = await _chatSessionRepository.GetMessagesAsLinkedListAsync(chatSessionId, userId, 40);

        //    // Chuyển lịch sử tin nhắn thành danh sách contents cho Gemini API
        //    var contents = chatMessages.Select(m => new GeminiAIDev.Models.Content
        //    {
        //        role = m.Role,
        //        parts = new[] { new GeminiAIDev.Models.Part { text = m.Content } }
        //    }).ToList();

        //    // Thêm tin nhắn mới của người dùng
        //    contents.Add(new GeminiAIDev.Models.Content
        //    {
        //        role = "user",
        //        parts = new[] { new GeminiAIDev.Models.Part { text = userMessage } }
        //    });

        //    // Gọi API Gemini với streaming
        //    string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:streamGenerateContent?alt=sse&key={_apiKey}";
        //    var request = new ContentRequest { contents = contents.ToArray() };
        //    string jsonRequest = JsonConvert.SerializeObject(request);
        //    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //    // Sử dụng SendAsync với HttpCompletionOption.ResponseHeadersRead
        //    using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
        //    {
        //        Content = content
        //    };
        //    using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        //    response.EnsureSuccessStatusCode();

        //    using var stream = await response.Content.ReadAsStreamAsync();
        //    using var reader = new StreamReader(stream);

        //    string aiResponse = "";
        //    while (!reader.EndOfStream)
        //    {
        //        var line = await reader.ReadLineAsync();
        //        if (!string.IsNullOrEmpty(line) && line.StartsWith("data: "))
        //        {
        //            var data = line.Substring(6); // Bỏ phần "data: "
        //            aiResponse += data;
        //            yield return data; // Trả về từng phần dữ liệu
        //        }
        //    }

        //    // Lưu tin nhắn vào MongoDB sau khi hoàn tất
        //    var userMessageEntity = new ChatMessage
        //    {
        //        Role = "user",
        //        Content = userMessage,
        //        SentAt = DateTime.Now
        //    };
        //    var aiMessageEntity = new ChatMessage
        //    {
        //        Role = "model",
        //        Content = aiResponse,
        //        SentAt = DateTime.Now
        //    };

        //    await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, userMessageEntity);
        //    await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, aiMessageEntity);
        //}
        public async IAsyncEnumerable<string> StreamGenerateContentAsync(int userId, string chatSessionId, string userMessage)
        {
            // Lấy lịch sử tin nhắn
            var chatMessages = await _chatSessionRepository.GetMessagesAsLinkedListAsync(chatSessionId, userId, 40);

            // Chuyển LinkedList thành danh sách contents cho Gemini API
            var contents = chatMessages.Select(m => new GeminiAIDev.Models.Content
            {
                role = m.Role,
                parts = new[]
                {
                new GeminiAIDev.Models.Part { text = m.Content }
            }
            }).ToList();

            // Thêm tin nhắn mới của người dùng
            contents.Add(new GeminiAIDev.Models.Content
            {
                role = "user",
                parts = new[] { new GeminiAIDev.Models.Part { text = userMessage } }
            });

            // Kiểm tra tổng độ dài (ước tính token)
            int estimatedTokenCount = contents.Sum(c => c.parts.Sum(p => p.text.Length / 4));
            if (estimatedTokenCount > 8000)
            {
                contents = contents.TakeLast(40).ToList();
            }

            // Gọi API Gemini với SSE
            string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:streamGenerateContent?alt=sse&key={_apiKey}";
            var request = new ContentRequest { contents = contents.ToArray() };
            string jsonRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            // Lưu tin nhắn của người dùng vào MongoDB
            var userMessageEntity = new ChatMessage
            {
                Role = "user",
                Content = userMessage,
                SentAt = DateTime.Now
            };
            await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, userMessageEntity);

            using var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };
            using var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream);

            StringBuilder fullResponse = new StringBuilder();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(":"))
                    continue;

                if (line.StartsWith("data:"))
                {
                    var jsonData = line["data:".Length..].Trim();
                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        var geminiResponse = JsonConvert.DeserializeObject<ContentResponse>(jsonData);
                        if (geminiResponse?.Candidates?.Length > 0)
                        {
                            var text = geminiResponse.Candidates[0].Content.Parts[0].Text;
                            fullResponse.Append(text);
                            yield return text; // Trả về từng phần khi nhận được
                        }
                    }
                }
            }

            // Lưu toàn bộ phản hồi của AI vào MongoDB sau khi stream kết thúc
            var aiMessageEntity = new ChatMessage
            {
                Role = "model",
                Content = fullResponse.ToString(),
                SentAt = DateTime.Now
            };
            await _chatSessionRepository.AddMessageAsync(chatSessionId, userId, aiMessageEntity);
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
