using FN.Application.Catalog.Product;
using FN.ViewModel.Catalog.Products.Manage;
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
        public PublicsController(IProductPublicService service)
        {
            _service = service;
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
    }
}
