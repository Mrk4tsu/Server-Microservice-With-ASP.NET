using FN.DataAccess.Entities;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace FN.AIService.Models
{
    public class ChatSession
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Lưu trữ tin nhắn dưới dạng danh sách
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
