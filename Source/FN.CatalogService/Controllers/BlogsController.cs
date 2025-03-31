using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.Comments;
using FN.Application.Catalog.Blogs.Interactions;
using FN.ProductService.Controllers;
using FN.ViewModel.Catalog.Blogs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FN.CatalogService.Controllers
{
    [Route("api/blog")]
    [ApiController, AllowAnonymous]
    public class BlogsController : BasesController
    {
        private readonly IBlogService _blogService;
        private readonly ITestRepository _testRepository;
        private readonly BlogInteraction _blogInteraction;
        public BlogsController(IBlogService blogService, BlogInteraction interaction, ITestRepository test)
        {
            _blogService = blogService;
            _blogInteraction = interaction;
            _testRepository = test;
        }
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var blog = await _blogService.GetDetail(id);
            return Ok(blog);
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetBlogs([FromQuery] BlogPagingRequest request)
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
        [HttpPut("update-view/{id}")]
        public async Task<IActionResult> UpdateView(int id)
        {
            var result = await _blogService.UpdateView(id);
            return Ok(result);
        }

        [HttpGet("list-test")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _testRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("test/{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var product = await _testRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("create-test")]
        public async Task<IActionResult> Create(TestProduct product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id = await _testRepository.AddAsync(product);
            return CreatedAtAction(nameof(Get), new { id }, product);
        }

        [HttpPut("update-test/{id}")]
        public async Task<IActionResult> Update(string id, TestProduct product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _testRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            await _testRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("delete-test/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _testRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            await _testRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
