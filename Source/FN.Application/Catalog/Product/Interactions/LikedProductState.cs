using FN.DataAccess;

namespace FN.Application.Catalog.Product.Interactions
{
    public class LikedProductState : IProductInteractionState
    {
        private AppDbContext _db;

        public LikedProductState(AppDbContext db)
        {
            _db = db;
        }
        public async Task HandleDislike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.LikeCount = productIn.LikeCount - 1;
            productIn.DislikeCount = productIn.DislikeCount + 1;
            await product.SetState(new DislikedProductState(_db), productId, userId);
        }

        public async Task HandleLike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.LikeCount = productIn.LikeCount - 1;
            await product.SetState(new NoInteractionProductState(_db), productId, userId);
        }
    }
}
