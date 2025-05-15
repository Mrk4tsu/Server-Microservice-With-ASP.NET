namespace FN.AIService.Models
{
    public class AssistantRequest
    {
        public string TypeChat { get; set; } = string.Empty;
        public string? SessionId { get; set; }
    }
}
