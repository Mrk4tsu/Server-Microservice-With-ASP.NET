using FN.DataAccess;

namespace FN.Application.Catalog.Product.Interactions
{
    public class DislikedProductState : IProductInteractionState
    {
        private AppDbContext _db;
        public DislikedProductState(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleDislike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.DislikeCount--;
            await product.SetState(new NoInteractionProductState(_db), productId, userId);
        }

        public async Task HandleLike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.LikeCount++;
            productIn.DislikeCount--;
            await product.SetState(new LikedProductState(_db), productId, userId);
        }
    }
}
