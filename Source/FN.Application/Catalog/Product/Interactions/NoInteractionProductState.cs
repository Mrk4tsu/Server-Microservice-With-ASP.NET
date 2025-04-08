using FN.DataAccess;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Product.Interactions
{
    public class NoInteractionProductState : IProductInteractionState
    {
        private AppDbContext _db;

        public NoInteractionProductState(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleDislike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.DislikeCount++;
            await product.Init(productId, InteractionType.Dislike, userId);
            await _db.SaveChangesAsync();
            await product.SetState(new DislikedProductState(_db), productId, userId);
        }

        public async Task HandleLike(ProductInteraction product, int productId, int userId)
        {
            var productIn = await _db.ProductDetails.FindAsync(productId);
            if (productIn == null) return;
            productIn.LikeCount++;
            await product.Init(productId, InteractionType.Like, userId);
            await _db.SaveChangesAsync();
            await product.SetState(new LikedProductState(_db), productId, userId);
        }
    }
}
