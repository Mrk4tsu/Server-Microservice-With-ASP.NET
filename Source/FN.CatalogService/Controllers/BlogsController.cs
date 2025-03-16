using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.Interactions;
using FN.ProductService.Controllers;
using FN.ViewModel.Catalog.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FN.CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController, AllowAnonymous]
    public class BlogsController : BasesController
    {
        private readonly IBlogService _blogService;
        private readonly BlogInteraction _blogInteraction;
        public BlogsController(IBlogService blogService, BlogInteraction interaction)
        {
            _blogService = blogService;
            _blogInteraction = interaction;
        }
        [HttpGet("blog/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var blog = await _blogService.GetDetail(id);
            return Ok(blog);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetBlogs([FromQuery] BlogPagingReques request)
        {
            var blogs = await _blogService.GetBlogs(request);
            return Ok(blogs);
        }
        [HttpPost("like/{blogId}")]
        public async Task<IActionResult> LikeBlog(int blogId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            await _blogInteraction.RestoreState(blogId, userId.Value);
            await _blogInteraction.PressLike(blogId, userId.Value);
            return Ok();
        }

        [HttpPost("dislike/{blogId}")]
        public async Task<IActionResult> DislikeBlog(int blogId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            await _blogInteraction.RestoreState(blogId, userId.Value);
            await _blogInteraction.PressDislike(blogId, userId.Value);
            return Ok();
        }
    }
}
