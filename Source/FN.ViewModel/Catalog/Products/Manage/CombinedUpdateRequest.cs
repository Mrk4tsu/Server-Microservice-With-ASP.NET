using FN.DataAccess.Enums;
using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Products.Manage
{
    public class CombinedUpdateRequest
    {
        // Các thuộc tính từ ItemUpdateDTO
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Keywords { get; set; }
        public IFormFile? Thumbnail { get; set; }

        // Các thuộc tính từ ProductDeatilUpdateRequest
        public string? Detail { get; set; }
        public string? Version { get; set; }
        public string? Note { get; set; }
        public byte CategoryId { get; set; }
        public ProductType Status { get; set; }

        //Cập nhật Image
        public List<IFormFile>? NewImages { get; set; }
    }
    public class DeleteProductImagesRequest
    {
        public List<int> ImageIds { get; set; }
    }
}
