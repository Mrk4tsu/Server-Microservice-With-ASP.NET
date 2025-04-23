namespace GeminiAIDev.Models
{
    public class ContentRequest
    {
        public Content[] contents { get; set; } = Array.Empty<Content>();
    }
    public class Content
    {
        public string role { get; set; } = string.Empty; // Thêm role: "user" hoặc "model"
        public Part[] parts { get; set; } = Array.Empty<Part>();
    }
    public class Part
    {
        public string text { get; set; } = string.Empty;
    }
}
