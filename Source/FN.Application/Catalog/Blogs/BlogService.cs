using FN.Application.Catalog.Blogs.Pattern;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Blogs
{
    public class BlogService : IBlogService
    {
        private string ROOT = "blog";
        private readonly AppDbContext _db;
        private readonly IImageService _image;
        private readonly IRedisService _redis;
        public BlogService(AppDbContext db,
            IImageService image,
            IRedisService redis)
        {
            _db = db;
            _image = image;
            _redis = redis;
        }
        string Folder(string code)
        {
            return $"{ROOT}/{code}";
        }
        public async Task<ApiResult<int>> CreateCombine(BlogCombineCreateRequest request, int userId)
        {
            var facade = new BlogCreateFacade(_db, _redis, _image);
            return await facade.CreateCombine(request, userId);
        }

        public async Task<ApiResult<PagedResult<BlogViewModel>>> GetBlogs(BlogPagingReques request)
        {
            var cacheKey = SystemConstant.CACHE_BLOG;
            List<BlogViewModel> data = new List<BlogViewModel>();
            if (await _redis.KeyExist(cacheKey))
            {
                data = await _redis.GetValue<List<BlogViewModel>>(cacheKey);
                if (data != null && data.Any() && data.Count > 0)
                    return new ApiSuccessResult<PagedResult<BlogViewModel>>(new PagedResult<BlogViewModel>
                    {
                        Items = data,
                        PageIndex = request.PageIndex,
                        PageSize = request.PageSize,
                        TotalRecords = data.Count
                    });
            }
            var blog = _db.Blogs.AsNoTracking()
                .Where(x => x.Item.IsDeleted == false)
                .Select(x => new BlogViewModel
                {
                    Id = x.Item.Id,
                    Author = x.Item.User.FullName,
                    Description = x.Item.Description,
                    SeoAlias = x.Item.SeoAlias,
                    Thumbnail = x.Item.Thumbnail,
                    TimeCreate = x.Item.CreatedDate,
                    Title = x.Item.Title,
                    ViewCount = x.Item.ViewCount,
                });
            var total = await blog.CountAsync();
            data = await blog
                .OrderByDescending(x => x.TimeCreate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            await _redis.SetValue(cacheKey, await blog.ToListAsync());
            var result = new PagedResult<BlogViewModel>
            {
                TotalRecords = total,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };
            return new ApiSuccessResult<PagedResult<BlogViewModel>>(result);
        }

        public async Task<ApiResult<BlogDetailViewModel>> GetDetail(int id)
        {
            var blog = await _db.Blogs
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.ItemId == id);
            if (blog == null) return new ApiErrorResult<BlogDetailViewModel>("Blog not found");
            var data = new BlogDetailViewModel
            {
                Id = blog.Item.Id,
                Author = blog.Item.User.FullName,
                Description = blog.Item.Description,
                SeoAlias = blog.Item.SeoAlias,
                Thumbnail = blog.Item.Thumbnail,
                TimeCreate = blog.Item.CreatedDate,
                Title = blog.Item.Title,
                ViewCount = blog.Item.ViewCount,
                SeoTitle = blog.Item.SeoTitle,
                LikeCount = blog.LikeCount,
                DislikeCount = blog.DislikeCount,
                Detail = blog.Detail,
                TimeUpdate = blog.Item.ModifiedDate,
            };
            return new ApiSuccessResult<BlogDetailViewModel>(data);
        }
    }
}
