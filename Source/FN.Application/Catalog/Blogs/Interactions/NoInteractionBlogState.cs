using FN.DataAccess;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Blogs.Interactions
{
    public class NoInteractionBlogState : IBlogInteractionState
    {
        private AppDbContext _db;

        public NoInteractionBlogState(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleDislike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.DislikeCount++;
            await blog.Init(blogId, InteractionType.Dislike, userId);
            await _db.SaveChangesAsync();
            await blog.SetState(new DislikedBlogState(_db), blogId, userId);
        }

        public async Task HandleLike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.LikeCount++;
            await blog.Init(blogId, InteractionType.Like, userId);
            await _db.SaveChangesAsync();
            await blog.SetState(new LikedBlogState(_db), blogId, userId);
        }
    }
}
