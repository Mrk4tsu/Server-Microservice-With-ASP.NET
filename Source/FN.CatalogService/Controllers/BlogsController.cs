using FN.Application.Catalog.Blogs;
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
        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromForm] BaseCreateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.CreateItem(request, userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("create-blog/{itemId}")]
        public async Task<IActionResult> CreateBlog(int itemId ,[FromForm] BlogCreateRequest request)
        {
            var result = await _blogService.CreateBlog(request, itemId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
