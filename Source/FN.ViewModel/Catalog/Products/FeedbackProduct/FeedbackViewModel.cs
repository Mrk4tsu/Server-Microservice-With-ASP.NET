namespace FN.ViewModel.Catalog.Products.FeedbackProduct
{
    public class FeedbackViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public byte Rate { get; set; }
        public DateTime TimeCreated { get; set; }
        public string Avatar { get; set; } = string.Empty;
    }
    public class FeedbackRequest
    {
        public int ProductId { get; set; }
        public string Content { get; set; } = string.Empty;
        public byte Rate { get; set; }
    }
}
