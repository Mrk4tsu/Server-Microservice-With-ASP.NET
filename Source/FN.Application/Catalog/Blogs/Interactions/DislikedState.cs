
using FN.DataAccess;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Blogs.Interactions
{
    public class DislikedState : IInteractionState
    {
        private AppDbContext _db;
        public DislikedState(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleDislike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.DislikeCount--;
            await blog.SetState(new NoInteractionState(_db), blogId, userId);
        }

        public async Task HandleLike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.LikeCount++;
            blogIn.DislikeCount--;
            await blog.SetState(new LikedState(_db), blogId, userId);
        }
    }
}
