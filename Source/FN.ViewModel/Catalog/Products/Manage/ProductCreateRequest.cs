using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.Http;

namespace FN.ViewModel.Catalog.Products.Manage
{
    public class ProductPagingRequest : PagedList
    {
        public string? KeyWord { get; set; }
        public int? CategoryId { get; set; }
        public string? CategorySeoAlias { get; set; }
    }
    public class CreateProductRequest : CreateImagesRequest
    {
        public string Detail { get; set; }
        public string Version { get; set; }
        public string Note { get; set; }
        public byte CategoryId { get; set; }
    }

    public class CreateImagesRequest : CreatePricesRequest
    {
        public IFormFileCollection Images { get; set; }
    }
    public class CreatePricesRequest : BaseCreateRequest
    {
        public decimal Price { get; set; }
    }
}
