using FN.AIService.Models;

namespace FN.AIService.Services.Chats
{
    public interface IChatSessionRepository
    {
        Task<ChatSession> GetChatSessionAsync(string sessionId, int userId);
        Task CreateChatSessionAsync(ChatSession session);
        Task AddMessageAsync(string sessionId, int userId, ChatMessage message);
        Task<LinkedList<ChatMessage>> GetMessagesAsLinkedListAsync(string sessionId, int userId);
        Task<LinkedList<ChatMessage>> GetMessagesAsLinkedListAsync(string sessionId, int userId, int maxMessages = 50);
        Task DeleteChatSessionAsync(string sessionId, int userId); // Thêm xóa session
        Task<List<ChatSession>> GetUserChatSessionsAsync(int userId, int page = 1, int pageSize = 10); // Thêm lấy danh sách session
    }
}
