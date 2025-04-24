namespace FN.AIService.Models
{
    public class GeminiStreamResponse
    {
        public List<GeminiCandidate> candidates { get; set; } = new();
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; } = new();
        public string finishReason { get; set; } = "";
    }

    public class GeminiContent
    {
        public List<GeminiPart> parts { get; set; } = new();
        public string role { get; set; } = "";
    }

    public class GeminiPart
    {
        public string text { get; set; } = "";
    }
}
