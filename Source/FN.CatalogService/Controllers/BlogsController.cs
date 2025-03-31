using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.BlogComments;
using FN.Application.Catalog.Blogs.Interactions;
using FN.ProductService.Controllers;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Catalog.Blogs.Comments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FN.CatalogService.Controllers
{
    [Route("api/blog")]
    [ApiController, AllowAnonymous]
    public class BlogsController : BasesController
    {
        private readonly IBlogService _blogService;
        private readonly IBlogCommentRepository _testRepository;
        private readonly BlogInteraction _blogInteraction;
        public BlogsController(IBlogService blogService, BlogInteraction interaction, IBlogCommentRepository test)
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

        [HttpGet("list-comment")]
        public async Task<IActionResult> GetAll()
        {
            var products = await _testRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("comment/{id}")]
        public async Task<IActionResult> Get(string id, int blogId)
        {
            var product = await _testRepository.GetByIdAsync(id, blogId);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost("comment")]
        public async Task<IActionResult> Create(BlogCommentCreate comment)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var id = await _testRepository.AddAsync(comment, userId.Value);
            return CreatedAtAction(nameof(Get), new { id }, comment);
        }

        [HttpPut("comment/{id}")]
        public async Task<IActionResult> Update(string id, BlogComment product, int blogId)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _testRepository.GetByIdAsync(id, blogId);
            if (existingProduct == null)
            {
                return NotFound();
            }

            await _testRepository.UpdateAsync(product);
            return NoContent();
        }

        [HttpDelete("comment/{id}")]
        public async Task<IActionResult> Delete(string id, int blogId)
        {
            var product = await _testRepository.GetByIdAsync(id, blogId);
            if (product == null)
            {
                return NotFound();
            }

            await _testRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
