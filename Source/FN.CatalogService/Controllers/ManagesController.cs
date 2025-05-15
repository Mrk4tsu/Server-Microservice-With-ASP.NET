using FN.Application.Catalog.Product;
using FN.Application.Catalog.Product.Prices;
using FN.Application.Helper.Images;
using FN.ViewModel.Catalog.ProductItems;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Catalog.Products.Prices;
using FN.ViewModel.Catalog.Products.Statistical;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ocelot.Values;
using System.Diagnostics;

namespace FN.ProductService.Controllers
{
    [Route("api/manage")]
    [ApiController, Authorize]
    public class ManagesController : BasesController
    {
        private readonly IProductManageService _service;
        private readonly IImageService _imageService;
        private readonly IPriceProductService _priceService;
        public ManagesController(IProductManageService service,
            IPriceProductService priceService,
            IImageService imageService)
        {
            this._service = service;
            this._imageService = imageService;
            _priceService = priceService;
        }
        [HttpGet("get/{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.GetProductById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
        [HttpGet("product-stats")]
        public async Task<IActionResult> GetUserProductsWithStats()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.GetUserProductsWithStats(userId.Value);
            return Ok(result);
        }
        [HttpGet("paging")]
        public async Task<IActionResult> GetProducts([FromQuery] ProductPagingRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.GetProducts(request, userId.Value);
            return Ok(result);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromForm] CombinedCreateOrUpdateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
            {
                return Unauthorized();
            }
            var result = await _service.Create(request, userId.Value);
            return Ok(result);
        }
        [HttpPut("update/{itemId}/{productId}")]
        public async Task<IActionResult> Update(int itemId, int productId, [FromForm] CombinedCreateOrUpdateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var result = await _service.Update(request, itemId, productId, userId.Value);
            if (result.Success) return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpPost("up-img"), AllowAnonymous]
        public async Task<IActionResult> UploadImage([FromForm] List<IFormFile> file, string folder)
        {
            var result = await _imageService.UploadImages(file, folder);
            if (result == null)
            {
                return Error("Upload image failed");
            }
            return Success(result);
        }
        [HttpDelete("image-remove")]
        public async Task<IActionResult> DeleteImage([FromBody] DeleteProductImagesRequest request)
        {
            var result = await _service.DeleteImage(request);
            if (!result.Success)
                return BadRequest();
            return Ok(result);
        }
        [HttpPost("add-price")]
        public async Task<IActionResult> AddPrice([FromForm] PriceRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _priceService.Create(request);
            return Ok(result);
        }
        [HttpPost("add-item/{productId}")]
        public async Task<IActionResult> AddItem([FromForm] ProductItemRequest request, int productId)
        {
            var result = await _service.AddItemProduct(request, productId);
            return Ok(result);
        }
        [HttpPut("edit-item/{itemProductID}")]
        public async Task<IActionResult> EditItem([FromForm] ProductItemSingleRequest request, int itemProductID)
        {
            var result = await _service.EditItemProduct(request, itemProductID);
            return Ok(result);
        }
        [HttpPut("update-price")]
        public async Task<IActionResult> UpdatePrice(PriceRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _priceService.Update(request);
            if (result.Success) return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpDelete("remove-price{id}")]
        public async Task<IActionResult> RemovePrice(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _priceService.Delete(id);
            if (result.Success) return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpGet("my-product")]
        public async Task<IActionResult> GetMyProducts([FromQuery] ProductPagingRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.GetProductsOwner(request, userId.Value);
            return Ok(result);
        }
        [HttpDelete("delete/{itemId}")]
        public async Task<IActionResult> DeleteOrRoleback(int itemId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.DeleteOrRoleback(itemId, userId.Value);
            if (result.Success) return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpDelete("delete-permanently/{itemId}")]
        public async Task<IActionResult> DeletePermanently(int itemId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.DeletePermanently(itemId, userId.Value);
            if (result.Success) return Ok(result);
            return BadRequest(result.Message);
        }
        [HttpGet("trash")]
        public async Task<IActionResult> TrashProducts([FromQuery] ProductPagingRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _service.TrashProducts(request, userId.Value);
            return Ok(result);
        }
    }
}
