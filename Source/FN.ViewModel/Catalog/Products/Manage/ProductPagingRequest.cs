using FN.ViewModel.Helper.Paging;

namespace FN.ViewModel.Catalog.Products.Manage
{
    public class ProductPagingRequest : PagedList
    {
        public string? KeyWord { get; set; }
        public int? CategoryId { get; set; }
        public string? CategorySeoAlias { get; set; }
    }
}
