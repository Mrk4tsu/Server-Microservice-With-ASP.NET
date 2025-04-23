namespace FN.AIService.ViewModels
{
    public class ChatSessionViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public int MessageCount { get; set; }
    }
}
