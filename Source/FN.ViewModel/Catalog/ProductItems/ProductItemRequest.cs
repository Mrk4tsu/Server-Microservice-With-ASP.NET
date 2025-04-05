using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.ProductItems
{
    public class ProductItemRequest
    {
        //public List<IFormFile>? File { get; set; }
        public List<string>? Url { get; set; }
    }
    public class ProductItemSingleRequest
    {
        //public IFormFile? File { get; set; }
        public string? Url { get; set; }
    }
}
