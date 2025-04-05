using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Blogs.Comments;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Identity;

namespace FN.Application.Catalog.Blogs.BlogComments
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly FirestoreDb _db;
        private string _collectionName = "comments";
        private readonly UserManager<AppUser> _userManager;
        public BlogCommentRepository(FirestoreDb db, UserManager<AppUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<BlogComment?> GetByIdAsync(string id, int blogId)
        {
            var snapshot = await _db.Collection($"{_collectionName}-{blogId}").Document(id).GetSnapshotAsync();
            return snapshot.Exists ? snapshot.ConvertTo<BlogComment?>() : null;
        }

        public async Task<IEnumerable<BlogComment>> GetAllAsync()
        {
            var snapshot = await _db.Collection(_collectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var product = doc.ConvertTo<BlogComment>();
                product.Id = doc.Id;
                return product;
            });
        }

        public async Task<string> AddAsync(BlogCommentCreate comment, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var newComment = new BlogComment
            {
                Id = Guid.NewGuid().ToString(),
                BlogId = comment.BlogId,
                UserId = userId,
                Content = comment.Content,
                CreatedDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
                LikeCount = 0,
                DislikeCount = 0,
                Avatar = user!.Avatar,
                UserName = user.UserName!,
                FullName = user.FullName,
                Status = true
            };
            var docRef = await _db.Collection($"{_collectionName}-{comment.BlogId}").AddAsync(newComment);
            return docRef.Id;
        }

        public async Task UpdateAsync(BlogComment product)
        {
            var docRef = _db.Collection(_collectionName).Document(product.Id);
            await docRef.SetAsync(product, SetOptions.MergeAll);
        }

        public async Task DeleteAsync(string id)
        {
            await _db.Collection(_collectionName).Document(id).DeleteAsync();
        }
    }
}
