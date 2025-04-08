using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Blogs.Interactions
{
    public class BlogInteraction
    {
        private IBlogInteractionState _currentState;
        private AppDbContext _db;
        public BlogInteraction(AppDbContext db)
        {
            _db = db;
            _currentState = new NoInteractionBlogState(_db);
        }
        public async Task SetState(IBlogInteractionState state, int blogId, int userId)
        {
            await Init(blogId, GetInteractionTypeFromState(state), userId);
            _currentState = state;
        }
        public async Task PressLike(int blogId, int userId)
        {
            await _currentState.HandleLike(this, blogId, userId);
        }
        public async Task PressDislike(int blogId, int userId)
        {
            await _currentState.HandleDislike(this, blogId, userId);
        }
        public async Task Init(int blogId, InteractionType type, int userId)
        {
            var interaction = await _db.UserBlogInteractions.FindAsync(userId, blogId);
            if (interaction == null)
            {
                interaction = new UserBlogInteraction()
                {
                    BlogId = blogId,
                    UserId = userId,
                    InteractionDate = DateTime.Now,
                    Type = InteractionType.None
                };
                _db.UserBlogInteractions.Add(interaction);
            }
            else
            {
                interaction.Type = type;
            }
            await _db.SaveChangesAsync();
        }
        private InteractionType GetInteractionTypeFromState(IBlogInteractionState state)
        {
            if (state is LikedBlogState)
                return InteractionType.Like;
            if (state is DislikedBlogState)
                return InteractionType.Dislike;
            return InteractionType.None;
        }
        public async Task RestoreState(int blogId, int userId)
        {
            var interaction = await _db.UserBlogInteractions.FindAsync(userId, blogId);
            if (interaction != null)
            {
                switch (interaction.Type)
                {
                    case InteractionType.Like:
                        _currentState = new LikedBlogState(_db);
                        break;
                    case InteractionType.Dislike:
                        _currentState = new DislikedBlogState(_db);
                        break;
                    default:
                        _currentState = new NoInteractionBlogState(_db);
                        break;
                }
            }
        }
    }
}
