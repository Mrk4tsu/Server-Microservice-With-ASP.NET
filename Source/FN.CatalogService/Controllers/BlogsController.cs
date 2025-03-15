using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.Interactions;
using FN.ProductService.Controllers;
using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FN.CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : BasesController
    {
        private readonly IBlogService _blogService;
        private readonly BlogInteraction _blogInteraction;
        public BlogsController(IBlogService blogService, BlogInteraction interaction)
        {
            _blogService = blogService;
            _blogInteraction = interaction;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] BlogCombineCreateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.CreateCombine(request, userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
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
