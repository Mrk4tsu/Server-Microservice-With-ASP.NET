using FN.AIService.Models;

namespace FN.AIService.Services.Chats
{
    public interface IChatSessionRepository
    {
        Task<ChatSession> GetChatSessionAsync(string sessionId, int userId);
        Task CreateChatSessionAsync(ChatSession session);
        Task AddMessageAsync(string sessionId, int userId, ChatMessage message);
        Task<LinkedList<ChatMessage>> GetMessagesAsLinkedListAsync(string sessionId, int userId);
    }
}
