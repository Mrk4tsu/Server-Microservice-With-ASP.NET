using FN.AIService.Models;
using MongoDB.Driver;

namespace FN.AIService.Services.Chats
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly IMongoCollection<ChatSession> _chatSessions;
        public ChatSessionRepository(IMongoDatabase database)
        {
            _chatSessions = database.GetCollection<ChatSession>("ChatSessions");
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
    }
}
