using FN.Forum.Models;
using FN.Forum.Services;
using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FN.Forum.Controllers
{
    [Route("api/topic")]
    [ApiController]
    public class TopicsController(ITopicService _postService, IReplyService _replyService) : BasesController
    {
        [HttpGet("list")]
        public async Task<IActionResult> GetPosts([FromQuery] PagedList request)
        {
            var result = await _postService.GetPosts(request);
            return Ok(result);
        }
        [HttpGet("get-post")]
        public async Task<IActionResult> GetPost(int id, [FromQuery] PagedList request)
        {
            var result = await _postService.GetPostById(id, request);
            return Ok(result);
        }
        [HttpGet("get-replies")]
        public async Task<IActionResult> GetReplies(int postId, [FromQuery] PagedList request)
        {
            var result = await _postService.GetRepliesByPostId(postId, request);
            return Ok(result);
        }
        [HttpPost("create-post"), Authorize]
        public async Task<IActionResult> CreatePost([FromBody] TopicRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _postService.CreatePost(request, userId.Value);
            return Ok(result);
        }
        [HttpPut("update-post"), Authorize]
        public async Task<IActionResult> UpdatePost(int id, [FromBody] TopicRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _postService.UpdatePost(id, request, userId.Value);
            return Ok(result);
        }
        [HttpDelete("delete-post"), Authorize]
        public async Task<IActionResult> DeletePost(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _postService.DeletePost(id, userId.Value);
            return Ok(result);
        }
        [HttpPost("create-reply"), Authorize]
        public async Task<IActionResult> CreateReply([FromBody] ReplyRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _replyService.CreateReply(request, userId.Value);
            return Ok(result);
        }
        [HttpPut("update-reply"), Authorize]
        public async Task<IActionResult> UpdateReply(int id, [FromBody] ReplyRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _replyService.UpdateReply(id, request, userId.Value);
            return Ok(result);
        }
        [HttpDelete("delete-reply"), Authorize]
        public async Task<IActionResult> DeleteReply(int id)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null)
                return Unauthorized();
            var result = await _replyService.DeleteReply(id, userId.Value);
            return Ok(result);
        }

    }
}
