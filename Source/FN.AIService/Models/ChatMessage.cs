namespace FN.AIService.Models
{
    public class ChatMessage
    {
        public string Role { get; set; } = string.Empty; // "user" hoặc "model"
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
