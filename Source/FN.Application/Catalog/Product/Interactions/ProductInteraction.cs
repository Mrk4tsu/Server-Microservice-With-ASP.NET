using FN.Application.Catalog.Blogs.Interactions;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Product.Interactions
{
    public class ProductInteraction
    {
        private IProductInteractionState _currentState;
        private AppDbContext _db;
        private DateTime _now;
        public ProductInteraction(AppDbContext db)
        {
            _db = db;
            _currentState = new NoInteractionProductState(_db);
            _now = new TimeHelper.Builder().SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time").Build();
        }
        public async Task SetState(IProductInteractionState state, int productId, int userId)
        {
            await Init(productId, GetInteractionTypeFromState(state), userId);
            _currentState = state;
        }
        public async Task PressLike(int productId, int userId)
        {
            await _currentState.HandleLike(this, productId, userId);
        }
        public async Task PressDislike(int productId, int userId)
        {
            await _currentState.HandleDislike(this, productId, userId);
        }
        public async Task Init(int productId, InteractionType type, int userId)
        {
            var interaction = await _db.UserProductInteractions.FindAsync(userId, productId);
            if (interaction == null)
            {
                interaction = new UserProductInteraction()
                {
                    ProductId = productId,
                    UserId = userId,
                    InteractionDate = _now,
                    Type = InteractionType.None
                };
                _db.UserProductInteractions.Add(interaction);
            }
            else
            {
                interaction.Type = type;
            }
            await _db.SaveChangesAsync();
        }
        private InteractionType GetInteractionTypeFromState(IProductInteractionState state)
        {
            if (state is LikedProductState)
                return InteractionType.Like;
            if (state is DislikedProductState)
                return InteractionType.Dislike;
            return InteractionType.None;
        }
        public async Task RestoreState(int blogId, int userId)
        {
            var interaction = await _db.UserProductInteractions.FindAsync(userId, blogId);
            if (interaction != null)
            {
                switch (interaction.Type)
                {
                    case InteractionType.Like:
                        _currentState = new LikedProductState(_db);
                        break;
                    case InteractionType.Dislike:
                        _currentState = new DislikedProductState(_db);
                        break;
                    default:
                        _currentState = new NoInteractionProductState(_db);
                        break;
                }
            }
        }
    }
}
