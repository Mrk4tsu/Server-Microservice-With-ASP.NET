using FN.DataAccess;
using FN.DataAccess.Enums;

namespace FN.Application.Catalog.Blogs.Interactions
{
    public class DislikedBlogState : IBlogInteractionState
    {
        private AppDbContext _db;
        public DislikedBlogState(AppDbContext db)
        {
            _db = db;
        }

        public async Task HandleDislike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.DislikeCount--;
            await blog.SetState(new NoInteractionBlogState(_db), blogId, userId);
        }

        public async Task HandleLike(BlogInteraction blog, int blogId, int userId)
        {
            var blogIn = await _db.Blogs.FindAsync(blogId);
            if (blogIn == null) return;
            blogIn.LikeCount++;
            blogIn.DislikeCount--;
            await blog.SetState(new LikedBlogState(_db), blogId, userId);
        }
    }
}
