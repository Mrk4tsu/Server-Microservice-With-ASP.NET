﻿using FN.Application.Catalog.Blogs.Interactions;
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
        public async Task<IActionResult> Create([FromForm] BlogCombineCreateOrUpdateRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.CreateCombine(request, userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("update/{itemId}/{blogId}")]
        public async Task<IActionResult> Update([FromForm] BlogCombineCreateOrUpdateRequest request, int itemId, int blogId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.UpdateCombine(request, itemId, blogId, userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.Delete(id, userId.Value);
            if (result.Success)
                return Ok(result);
            return Error(result.Message);
        }
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _blogService.DeletePermanently(id, userId.Value);
            if (result.Success)
                return Ok(result);
            return Error(result.Message);
        }
    }
}
