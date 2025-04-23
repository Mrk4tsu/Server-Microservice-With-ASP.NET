using FN.AIService.Models;
using FN.AIService.ViewModels;
using FN.ViewModel.Helper.API;
using MongoDB.Driver;

namespace FN.AIService.Services.Chats
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly IMongoCollection<ChatSession> _chatSessions;
        public ChatSessionRepository(IMongoDatabase database)
        {
            _chatSessions = database.GetCollection<ChatSession>("ChatSessions");
            CreateIndexes();
        }
        public async Task AddMessageAsync(string sessionId, int userId, ChatMessage message)
        {
            var filter = Builders<ChatSession>.Filter
            .Where(s => s.Id == sessionId && s.UserId == userId);
            var update = Builders<ChatSession>.Update
                .Push(s => s.Messages, message)
                .Set(s => s.LastModifiedDate, DateTime.Now);

            await _chatSessions.UpdateOneAsync(filter, update);
        }

        public async Task CreateChatSessionAsync(ChatSession session)
        {
            await _chatSessions.InsertOneAsync(session);
        }

        public async Task<ChatSession> GetChatSessionAsync(string sessionId, int userId)
        {
            return await _chatSessions
            .Find(s => s.Id == sessionId && s.UserId == userId)
            .FirstOrDefaultAsync();
        }

        public async Task<LinkedList<ChatMessage>> GetMessagesAsLinkedListAsync(string sessionId, int userId)
        {
            var session = await GetChatSessionAsync(sessionId, userId);
            return session != null ? new LinkedList<ChatMessage>(session.Messages) : new LinkedList<ChatMessage>();
        }
        public async Task<LinkedList<ChatMessage>> GetMessagesAsLinkedListAsync(string sessionId, int userId, int maxMessages = 50)
        {
            var session = await _chatSessions
                .Find(s => s.Id == sessionId && s.UserId == userId)
                .FirstOrDefaultAsync();

            if (session == null)
                return new LinkedList<ChatMessage>();

            // Lấy maxMessages tin nhắn gần nhất, sắp xếp theo SentAt
            var messages = session.Messages
                .OrderByDescending(m => m.SentAt)
                .Take(maxMessages)
                .OrderBy(m => m.SentAt) // Đảm bảo thứ tự thời gian
                .ToList();

            return new LinkedList<ChatMessage>(messages);
        }
        public async Task DeleteChatSessionAsync(string sessionId, int userId)
        {
            var filter = Builders<ChatSession>.Filter
                .Where(s => s.Id == sessionId && s.UserId == userId);
            var result = await _chatSessions.DeleteOneAsync(filter);

            if (result.DeletedCount == 0)
                throw new Exception("Chat session not found or unauthorized.");
        }
        public async Task<ApiResult<List<ChatSessionViewModel>>> GetUserChatSessionsAsync(int userId, int page = 1, int pageSize = 10)
        {
            try
            {
                var sessions = await _chatSessions
               .Find(s => s.UserId == userId && s.IsActive)
               .SortByDescending(s => s.LastModifiedDate)
               .Skip((page - 1) * pageSize)
               .Limit(pageSize)
               .ToListAsync();
                var sessionDtos = sessions.Select(s => new ChatSessionViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    CreatedDate = s.CreatedDate,
                    LastModifiedDate = s.LastModifiedDate,
                    MessageCount = s.Messages.Count
                }).ToList();
                return new ApiSuccessResult<List<ChatSessionViewModel>>(sessionDtos);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<List<ChatSessionViewModel>>(ex.Message);
            }
        }
        private void CreateIndexes()
        {
            var indexKeys = Builders<ChatSession>.IndexKeys
                .Ascending(s => s.UserId)
                .Ascending(s => s.IsActive);

            var indexModel = new CreateIndexModel<ChatSession>(
                indexKeys,
                new CreateIndexOptions { Background = true } // Tạo index không chặn hoạt động khác
            );

            _chatSessions.Indexes.CreateOne(indexModel);
        }
    }
}
