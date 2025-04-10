namespace FN.ViewModel.Catalog.Products.Manage
{
    public class ManageProductViewModel
    {
        public int Id { get; set; }
        // Các thuộc tính từ ItemUpdateDTO
        public string Title { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public string Thumbnail { get; set; }

        // Các thuộc tính từ ProductDeatilUpdateRequest
        public string Detail { get; set; }
        public string Version { get; set; }
        public string Note { get; set; }
        public byte CategoryId { get; set; }
        //Cập nhật Image
        public List<ImageProductViewModel> Images { get; set; }
        //Url
        public List<UrlProductViewModel> Url { get; set; }
        public decimal Price { get; set; }
    }
}
