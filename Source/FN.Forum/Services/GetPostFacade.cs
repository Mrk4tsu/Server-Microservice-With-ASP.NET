using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Forum.Models;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;

namespace FN.Forum.Services
{
    public class GetPostFacade
    {
        private readonly AppDbContext _db;
        private readonly IRedisService _redis;
        public GetPostFacade(AppDbContext db, IRedisService redis)
        {
            _db = db;
            _redis = redis;
        }
        public async Task<ApiResult<PagedResult<TopicViewModel>>> GetPosts(PagedList request)
        {
            const string cacheKey = SystemConstant.CACHE_POST;
            List<TopicViewModel>? cachedData = null;
            bool useCache = await _redis.KeyExist(cacheKey);

            if (useCache)
            {
                cachedData = await _redis.GetValue<List<TopicViewModel>>(cacheKey);
            }

            PagedResult<TopicViewModel> result;

            if (useCache && cachedData != null && cachedData.Count > 0)
            {
                var filteredData = ApplyMemoryFilters(cachedData, request);
                result = CreatePagedResult(filteredData, request);
            }
            else
            {
                var query = BuildBaseQuery();
                var filteredQuery = ApplyDatabaseFilters(query, request);
                result = await ExecuteDatabasePaging(filteredQuery, request);

                await CacheBaseData(query, cacheKey);
            }

            return new ApiSuccessResult<PagedResult<TopicViewModel>>(result);
        }
        private IQueryable<TopicViewModel> BuildBaseQuery()
        {
            return _db.Topics.Include(x => x.User)
                .Where(x => !x.IsDeleted)
                .Select(x => new TopicViewModel
                {
                    Id = x.Id,
                    AuthorDisplayName = x.User.UserName!,
                    AuthorAvatarUrl = x.User.Avatar,
                    CreatedAt = x.CreatedAt,
                    AuthorId = x.UserId,
                    CommentCount = _db.Replies.Count(r => r.TopicId == x.Id),
                    Title = x.Title,
                    UpdatedAt = x.UpdatedAt,
                    IsLocked = x.IsLocked
                });
        }
        private List<TopicViewModel> ApplyMemoryFilters(List<TopicViewModel> data, PagedList request)
        {
            return data.OrderByDescending(x => x.UpdatedAt).ToList();
        }
        private IQueryable<TopicViewModel> ApplyDatabaseFilters(IQueryable<TopicViewModel> query, PagedList request)
        {
            return query.OrderByDescending(x => x.UpdatedAt);
        }
        private async Task<PagedResult<TopicViewModel>> ExecuteDatabasePaging(IQueryable<TopicViewModel> query, PagedList request)
        {
            var total = await query.CountAsync();
            var items = await query
                 .Skip((request.PageIndex - 1) * request.PageSize)
                 .Take(request.PageSize)
                 .ToListAsync();

            return new PagedResult<TopicViewModel>
            {
                TotalRecords = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = items
            };
        }
        private PagedResult<TopicViewModel> CreatePagedResult(List<TopicViewModel> data, PagedList request)
        {
            return new PagedResult<TopicViewModel>
            {
                TotalRecords = data.Count,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList()
            };
        }
        private async Task CacheBaseData(IQueryable<TopicViewModel> query, string cacheKey)
        {
            var cacheData = await query
                .OrderByDescending(x => x.UpdatedAt)
                .ToListAsync();

            await _redis.SetValue(cacheKey, cacheData);
        }
    }
}
