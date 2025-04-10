using FN.ViewModel.Catalog.Blogs.Comments;

namespace FN.Application.Catalog.Blogs.BlogComments
{
    public interface IBlogCommentRepository
    {
        Task<BlogComment?> GetByIdAsync(string id, int blogId);
        Task<IEnumerable<BlogComment>> GetAllAsync();
        Task<string> AddAsync(BlogCommentCreate comment, int userId);
        Task UpdateAsync(BlogComment product);
        Task DeleteAsync(string id);
    }
}
