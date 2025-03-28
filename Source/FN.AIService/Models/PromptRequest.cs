namespace GeminiAIDev.Models
{
    public class PromptRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
    public class PromptDescriptionRequest
    {
        public string Name { get; set; } = string.Empty;
        public string System { get; set; } = string.Empty;
        public string Other { get; set; } = string.Empty;
    }
    public class PostResponse
    {
        public string Title { get; set; } = string.Empty;
        public string Thumbnail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DetailHtml { get; set; } = string.Empty;
        public DateTime PublishedDate { get; set; }
    }
}
