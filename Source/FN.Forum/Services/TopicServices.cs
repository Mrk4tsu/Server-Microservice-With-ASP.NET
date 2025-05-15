using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Forum.Models;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;

namespace FN.Forum.Services
{
    public interface ITopicService
    {
        Task<ApiResult<PagedResult<TopicViewModel>>> GetPosts(PagedList request);
        Task<ApiResult<TopicDetailViewModel>> GetPostById(int id, PagedList request);
        Task<ApiResult<PagedResult<ReplyViewModel>>> GetRepliesByPostId(int postId, PagedList request);
        Task<ApiResult<int>> CreatePost(TopicRequest request, int userId);
        Task<ApiResult<bool>> UpdatePost(int id, TopicRequest request, int userId);
        Task<ApiResult<bool>> UpdatePostStatus(int id, int userId);
        Task<ApiResult<bool>> DeletePost(int id, int userId);
    }
    public class TopicServices : ITopicService
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redis;
        private DateTime _now;
        public TopicServices(AppDbContext db, IRedisService redisService)
        {
            _db = db;
            _redis = redisService;
            _now = new TimeHelper.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
        }

        public async Task<ApiResult<int>> CreatePost(TopicRequest request, int userId)
        {
            var post = new Topic
            {
                Content = request.Content,
                Title = request.Title,
                UserId = userId,
                CreatedAt = _now,
                UpdatedAt = _now,
                IsDeleted = false,
                IsLocked = false,
            };
            _db.Topics.Add(post);
            await _db.SaveChangesAsync();
            await RemoveOldCache();
            return new ApiSuccessResult<int>(post.Id);
        }

        public async Task<ApiResult<bool>> DeletePost(int id, int userId)
        {
            var post = await _db.Topics.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (post == null) return new ApiErrorResult<bool>("Không có quyền để xóa bài viết này");
            post.IsDeleted = true;
            _db.Topics.Update(post);
            await _db.SaveChangesAsync();
            await RemoveOldCache();
            return new ApiSuccessResult<bool>(true);
        }
        public async Task<ApiResult<TopicDetailViewModel>> GetPostById(int id, PagedList request)
        {
            var post = await _db.Topics.Include(x => x.User).FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == id);
            if (post == null) return new ApiErrorResult<TopicDetailViewModel>("Post not found or has been deleted");

            var postViewModel = new TopicDetailViewModel
            {
                Id = post.Id,
                AuthorDisplayName = post.User.UserName!,
                AuthorAvatarUrl = post.User.Avatar,
                CreatedAt = post.CreatedAt,
                AuthorId = post.UserId,
                Content = post.Content,
                IsLocked = post.IsLocked,
                Title = post.Title,
            };
            return new ApiSuccessResult<TopicDetailViewModel>(postViewModel);
        }

        public async Task<ApiResult<PagedResult<TopicViewModel>>> GetPosts(PagedList request)
        {
            var postFacade = new GetPostFacade(_db, _redis);
            var result = await postFacade.GetPosts(request);
            return result;
        }

        public async Task<ApiResult<PagedResult<ReplyViewModel>>> GetRepliesByPostId(int postId, PagedList request)
        {
            var query = _db.Replies.Include(x => x.User).Where(x => x.TopicId == postId && !x.IsDeleted).AsQueryable();
            var totalRow = await query.CountAsync();
            var replies = await query.OrderByDescending(x => x.CreatedAt)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ReplyViewModel
                {
                    Id = x.Id,
                    AuthorDisplayName = x.User.UserName!,
                    AuthorAvatarUrl = x.User.Avatar,
                    CreatedAt = x.CreatedAt,
                    AuthorId = x.UserId,
                    Content = x.Content,
                    ParentReplyId = x.ParentId
                }).ToListAsync();
            var pageResult = new PagedResult<ReplyViewModel>
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = replies
            };
            return new ApiSuccessResult<PagedResult<ReplyViewModel>>(pageResult);
        }

        public async Task<ApiResult<bool>> UpdatePost(int id, TopicRequest request, int userId)
        {
            var post = await _db.Topics.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (post == null) return new ApiErrorResult<bool>("Post not found");
            post.Content = request.Content;
            post.Title = request.Title;
            post.UpdatedAt = _now;
            _db.Topics.Update(post);
            await _db.SaveChangesAsync();
            await RemoveOldCache();
            return new ApiSuccessResult<bool>(true);
        }

        public async Task<ApiResult<bool>> UpdatePostStatus(int id, int userId)
        {
            var adminRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId && x.RoleId == 1);
            var author = await _db.Topics.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (adminRole == null || author == null) return new ApiErrorResult<bool>("Bạn không có quyền để thực hiện hành động này");
            var post = await _db.Topics.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null) return new ApiErrorResult<bool>("Post not found");
            post.IsLocked = !post.IsLocked;
            _db.Topics.Update(post);
            await _db.SaveChangesAsync();
            await RemoveOldCache();
            return new ApiSuccessResult<bool>(true);
        }
        private async Task RemoveOldCache()
        {
            await _redis.RemoveValue(SystemConstant.CACHE_POST);
        }
    }
}
