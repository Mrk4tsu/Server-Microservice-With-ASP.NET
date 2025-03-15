
using FN.DataAccess;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Blogs.Interactions
{
    public class LikedState : IInteractionState
    {
        private AppDbContext _db;

        public LikedState(AppDbContext db)
        {
            _db = db;
        }
        public async Task HandleDislike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.LikeCount = blogIn.LikeCount - 1;
            blogIn.DislikeCount = blogIn.DislikeCount + 1;
            await blog.SetState(new DislikedState(_db), blogId, userId);
        }

        public async Task HandleLike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.LikeCount = blogIn.LikeCount - 1;
            await blog.SetState(new NoInteractionState(_db), blogId, userId);
        }
    }
}
