using FN.Application.Catalog.Blogs.Interactions;
using FN.Application.Catalog.Product;
using FN.Application.Catalog.Product.Interactions;
using FN.ViewModel.Catalog.Products.FeedbackProduct;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FN.ProductService.Controllers
{
    [Route("api/public")]
    [ApiController, AllowAnonymous]
    public class PublicsController : BasesController
    {
        private readonly IProductPublicService _service;
        private readonly ProductInteraction _productInteraction;
        public PublicsController(IProductPublicService service, ProductInteraction interaction)
        {
            _service = service;
            _productInteraction = interaction;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductPagingRequest request)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var products = await _service.GetProducts(request);
            Console.WriteLine($"Time to get products: {stopwatch.ElapsedMilliseconds}ms");
            stopwatch.Stop();
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductGetProductWithoutLogin(int id)
        {
            var product = await _service.GetProductWithoutLogin(id);
            return Ok(product);
        }
        [HttpGet("product/{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var product = await _service.GetProduct(id, userId.Value);
            return Ok(product);
        }
        [HttpPost("like/{productId}")]
        public async Task<IActionResult> LikeProduct(int productId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            await _productInteraction.RestoreState(productId, userId.Value);
            await _productInteraction.PressLike(productId, userId.Value);
            return Success(true);
        }

        [HttpPost("dislike/{productId}")]
        public async Task<IActionResult> DislikeProduct(int productId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            await _productInteraction.RestoreState(productId, userId.Value);
            await _productInteraction.PressDislike(productId, userId.Value);
            return Success(true);
        }
        [HttpGet("list-feedback")]
        public async Task<IActionResult> GetFeedbacks([FromQuery] PagedList request, int productId)
        {
            var result = await _service.GetFeedbackProduct(request, productId);
            return Ok(result);
        }
        [HttpPost("add-feedback")]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.AddProductFeedback(request, userId.Value);
            return Ok(result);
        }
        [HttpGet("selection")]
        public async Task<IActionResult> GetProductsSelection([FromQuery] string type, int take)
        {
            var result = await _service.GetProducts(type, take);
            return Ok(result);
        }
        [HttpPut("update-view")]
        public async Task<IActionResult> UpdateView(int productId)
        {
            var result = await _service.UpdateView(productId);
            return Ok(result);
        }
        [HttpGet("get-metadata")]
        public async Task<IActionResult> GetOpenGraph(int productId)
        {
            var result = await _service.GetOpenGraph(productId);
            return Ok(result);
        }
    }
}
