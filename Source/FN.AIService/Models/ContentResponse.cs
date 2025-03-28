namespace GeminiAIDev.Models.ContentResponse
{
    internal sealed class ContentResponse
    {
        public Candidate[] Candidates { get; set; } = Array.Empty<Candidate>();
        public PromptFeedback PromptFeedback { get; set; } = new();
    }

    internal sealed class PromptFeedback
    {
        public SafetyRating[] SafetyRatings { get; set; } = Array.Empty<SafetyRating>();
    }

    internal sealed class Candidate
    {
        public Content Content { get; set; } = new();
        public string FinishReason { get; set; } = string.Empty;
        public int Index { get; set; }
        public SafetyRating[] SafetyRatings { get; set; } = Array.Empty<SafetyRating>();
    }

    internal sealed class Content
    {
        public Part[] Parts { get; set; } = Array.Empty<Part>();
        public string Role { get; set; } = string.Empty;    
    }

    internal sealed class Part
    {
        // This one interests us the most
        public string Text { get; set; } = string.Empty;
    }

    internal sealed class SafetyRating
    {
        public string Category { get; set; } = string.Empty;
        public string Probability { get; set; } = string.Empty;
    }
}
