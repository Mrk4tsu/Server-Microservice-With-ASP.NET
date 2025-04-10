namespace FN.Application.Catalog.Blogs.Interactions
{
    public interface IBlogInteractionState
    {
        Task HandleLike(BlogInteraction blog, int blogId, int userId);
        Task HandleDislike(BlogInteraction blog, int blogId, int userId);
    }
}
