using FN.Application.Catalog.Blogs.Interactions;
using FN.Application.Catalog.Blogs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FN.ViewModel.Catalog.Blogs;
using FN.ProductService.Controllers;
using Microsoft.AspNetCore.Authorization;

namespace FN.CatalogService.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class ManageBlogsController : BasesController
    {
        private readonly IBlogService _blogService;
        public ManageBlogsController(IBlogService blogService)
        {
            _blogService = blogService;
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
    }
}
