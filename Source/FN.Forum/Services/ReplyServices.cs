using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Forum.Models;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;

namespace FN.Forum.Services
{
    public interface IReplyService
    {
        Task<ApiResult<int>> CreateReply(ReplyRequest request, int userId);
        Task<ApiResult<bool>> UpdateReply(int id, ReplyRequest request, int userId);
        Task<ApiResult<bool>> DeleteReply(int id, int userId);
        Task<ApiResult<ReplyViewModel>> GetReplyById(int id);
        Task<ApiResult<PagedResult<ReplyViewModel>>> GetReplies(int postId, PagedList request);
    }
    public class ReplyServices : IReplyService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redis;
        private DateTime _now;
        public ReplyServices(AppDbContext db, IRedisService redis)
        {
            _db = db;
            _now = new TimeHelper.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
            _redis = redis;
        }
        public async Task<ApiResult<int>> CreateReply(ReplyRequest request, int userId)
        {
            var post = await _db.Topics.FirstOrDefaultAsync(x => x.Id == request.PostId && !x.IsDeleted && !x.IsLocked);
            if (post == null) return new ApiErrorResult<int>("Post not found or has been deleted");
            var newReply = new Reply
            {
                Content = request.Content,
                TopicId = request.PostId,
                UserId = userId,
                CreatedAt = _now,
                UpdatedAt = _now,
                IsDeleted = false,
            };
            _db.Replies.Add(newReply);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newReply.Id);
        }

        public async Task<ApiResult<bool>> DeleteReply(int id, int userId)
        {
            var reply = _db.Replies.Include(x => x.User).FirstOrDefault(x => x.Id == id && !x.IsDeleted && x.UserId == userId);
            if (reply == null) return new ApiErrorResult<bool>("Reply not found or has been deleted");
            reply.IsDeleted = true;
            _db.Replies.Remove(reply);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true);
        }

        public Task<ApiResult<PagedResult<ReplyViewModel>>> GetReplies(int postId, PagedList request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResult<ReplyViewModel>> GetReplyById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<ApiResult<bool>> UpdateReply(int id, ReplyRequest request, int userId)
        {
            var reply = _db.Replies.FirstOrDefault(x => x.Id == id && !x.IsDeleted && x.UserId == userId);
            if (reply == null) return new ApiErrorResult<bool>("Reply not found or has been deleted");
            reply.Content = request.Content;
            reply.UpdatedAt = _now;
            _db.Replies.Update(reply);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<bool>(true);

        }
    }
}
