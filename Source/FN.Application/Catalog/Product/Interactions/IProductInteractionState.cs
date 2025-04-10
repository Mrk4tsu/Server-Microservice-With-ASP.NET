
namespace FN.Application.Catalog.Product.Interactions
{
    public interface IProductInteractionState
    {
        Task HandleLike(ProductInteraction product, int productId, int userId);
        Task HandleDislike(ProductInteraction product, int productId, int userId);
    }
}
