namespace GeminiAIDev.Models
{
    public class ContentRequest
    {
        public Content[] contents { get; set; } = Array.Empty<Content>();
    }
    public class Content
    {
        public Part[] parts { get; set; } = Array.Empty<Part>();
    }
    public class Part
    {
        public string text { get; set; } = string.Empty;
    }
}
