using AutoMapper;
using FN.Application.Catalog.Blogs.Pattern;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Blogs
{
    public class BlogService : IBlogService
    {
        private readonly AppDbContext _db;
        private readonly IImageService _image;
        private readonly IRedisService _redis;
        private readonly IMapper _mapper;
        public BlogService(AppDbContext db,
            IImageService image,
            IMapper mapper,
            IRedisService redis)
        {
            _db = db;
            _image = image;
            _redis = redis;
            _mapper = mapper;
        }
        string Folder(string code)
        {
            return $"{SystemConstant.BLOG_KEY}/{code}";
        }
        public async Task<ApiResult<int>> CreateCombine(BlogCombineCreateOrUpdateRequest request, int userId)
        {
            var facade = new BlogCreateFacade(_db, _redis, _image);
            return await facade.CreateCombine(request, userId);
        }
        public async Task<ApiResult<int>> UpdateCombine(BlogCombineCreateOrUpdateRequest request, int itemId, int blogId, int userId)
        {
            var facade = new BlogUpdateFacade(_db, _redis, _image, SystemConstant.BLOG_KEY);
            return await facade.Update(request, itemId, blogId, userId);
        }
        public async Task<ApiResult<List<BlogViewModel>>> GetLatestBlogs()
        {
            var cacheKey = SystemConstant.BLOG_KEY + "new";
            if (await _redis.KeyExist(cacheKey))
            {
                var cachedData = await _redis.GetValue<List<BlogViewModel>>(cacheKey);
                if (cachedData != null) return new ApiSuccessResult<List<BlogViewModel>>(cachedData);
            }
            var query = _db.Blogs.AsNoTracking()
                .Where(x => !x.Item.IsDeleted)
                .Include(x => x.Item)
                .ThenInclude(x => x.User);
            var blogs = await query
                .OrderByDescending(x => x.Item.CreatedDate)
                .Take(2)
                .ToListAsync();
            var data = _mapper.Map<List<BlogViewModel>>(blogs);
            await _redis.SetValue(cacheKey, data, TimeSpan.FromHours(12));
            return new ApiSuccessResult<List<BlogViewModel>>(data);
        }
        public async Task<ApiResult<PagedResult<BlogViewModel>>> GetMyBlogs(BlogPagingRequest request, int userId)
        {
            var cachePageKey = $"my{SystemConstant.BLOG_KEY}:{userId}";

            // Kiểm tra cache
            if (await _redis.KeyExist(cachePageKey))
            {
                var cachedData = await _redis.GetValue<List<BlogViewModel>>(cachePageKey);
                var resultCache = new PagedResult<BlogViewModel>
                {
                    TotalRecords = cachedData.Count,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    Items = cachedData
                };
                if (cachedData != null) return new ApiSuccessResult<PagedResult<BlogViewModel>>(resultCache);
            }

            // Truy vấn tổng số blog trước khi phân trang
            var query = _db.Blogs.AsNoTracking()
                .Where(x => !x.Item.IsDeleted && x.Item.UserId == userId)
                .Include(x => x.Item)
                .ThenInclude(x => x.User);

            var totalRecords = await query.Select(x => x.Id).CountAsync();

            // Truy vấn danh sách blog đã phân trang
            var blogs = await query
                .OrderByDescending(x => x.Item.CreatedDate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Ánh xạ dữ liệu bằng AutoMapper
            var data = _mapper.Map<List<BlogViewModel>>(blogs);
            // Lưu cache
            await _redis.SetValue(cachePageKey, data);

            var result = new PagedResult<BlogViewModel>
            {
                TotalRecords = totalRecords,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<BlogViewModel>>(result);
        }
        public async Task<ApiResult<PagedResult<BlogViewModel>>> GetBlogs(BlogPagingRequest request)
        {
            //var cachePageKey = SystemConstant.BLOG_KEY;

            //// Kiểm tra cache
            //if (await _redis.KeyExist(cachePageKey))
            //{
            //    var cachedData = await _redis.GetValue<List<BlogViewModel>>(cachePageKey);
            //    var resultCache = new PagedResult<BlogViewModel>
            //    {
            //        TotalRecords = cachedData!.Count,
            //        PageIndex = request.PageIndex,
            //        PageSize = request.PageSize,
            //        Items = cachedData
            //    };
            //    if (cachedData != null) return new ApiSuccessResult<PagedResult<BlogViewModel>>(resultCache);
            //}

            // Truy vấn tổng số blog trước khi phân trang
            var query = _db.Blogs.AsNoTracking()
                .Where(x => !x.Item.IsDeleted)
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.KeyWord))
                query = query.Where(x => x.Item.Title.Contains(request.KeyWord) 
                || x.Item.NormalizedTitle.Contains(request.KeyWord)
                || x.Item.User.FullName.Contains(request.KeyWord));

            var totalRecords = await query.Select(x => x.Id).CountAsync();

            // Truy vấn danh sách blog đã phân trang
            var blogs = await query
                .OrderByDescending(x => x.Item.CreatedDate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            // Ánh xạ dữ liệu bằng AutoMapper
            var data = _mapper.Map<List<BlogViewModel>>(blogs);

            // Lưu cache
            //await _redis.SetValue(cachePageKey, data);

            var result = new PagedResult<BlogViewModel>
            {
                TotalRecords = totalRecords,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = data
            };

            return new ApiSuccessResult<PagedResult<BlogViewModel>>(result);
        }
        public async Task<ApiResult<BlogDetailViewModel>> GetDetail(int id, int userId)
        {
            var blog = await _db.Blogs
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (blog == null || blog.Item == null || blog.Item.User == null)
                return new ApiErrorResult<BlogDetailViewModel>("Blog not found");

            var data = _mapper.Map<BlogDetailViewModel>(blog);
            var interaction = await _db.UserBlogInteractions
                .Where(x => x.BlogId == id && x.UserId == userId)
                .FirstOrDefaultAsync();
            if (interaction != null)
            {
                data.IsInteractive = interaction.Type;
            }

            return new ApiSuccessResult<BlogDetailViewModel>(data);
        }
        public async Task<ApiResult<BlogDetailViewModel>> GetDetailWithoutLogin(int id)
        {
            var blog = await _db.Blogs
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (blog == null || blog.Item == null || blog.Item.User == null)
                return new ApiErrorResult<BlogDetailViewModel>("Blog not found");

            var data = _mapper.Map<BlogDetailViewModel>(blog);
            data.IsInteractive = InteractionType.None;

            return new ApiSuccessResult<BlogDetailViewModel>(data);
        }
        public async Task<ApiResult<bool>> Delete(int itemId, int userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId);
            if (item == null) return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");

            item.IsDeleted = true;

            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            await RemoveCacheData(null);
            return new ApiSuccessResult<bool>();
        }
        public async Task<ApiResult<bool>> DeletePermanently(int itemId, int userId)
        {
            var itemToDel = await _db.Items
               .Where(x => x.Id == itemId)
               .Select(x => new
               {
                   x.Id,
                   x.Code,
                   Blogs = x.Blogs!.Select(pd => new
                   {
                       pd.Id,
                       BlogImages = pd.BlogImages.Select(pi => pi.PublicId).ToList()
                   }).ToList()
               })
               .FirstOrDefaultAsync();

            if (itemToDel == null)
                return new ApiErrorResult<bool>("Không tìm thấy Blog");

            var folder = Folder(itemToDel.Id.ToString());

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var deleteImageTasks = new List<Task>();

                deleteImageTasks.Add(_image.DeleteImageInFolder(itemToDel.Id.ToString(), folder)); // Xóa thumbnail

                foreach (var blogToDel in itemToDel.Blogs)
                {
                    foreach (var imageId in blogToDel.BlogImages)
                    {
                        deleteImageTasks.Add(_image.DeleteImageInFolder(imageId, $"{folder}/assets"));
                    }
                }
                await Task.WhenAll(deleteImageTasks);
                await _image.DeleteFolderImage(folder);
                var productDetails = await _db.Blogs
                    .Where(pd => pd.ItemId == itemToDel.Id)
                    .ToListAsync();

                _db.Blogs.RemoveRange(productDetails);

                var item = await _db.Items
                    .Where(x => x.Id == itemToDel.Id)
                    .FirstOrDefaultAsync();

                if (item != null)
                {
                    _db.Items.Remove(item);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                await RemoveCacheData(null);
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Xóa sản phẩm thất bại", ex);
            }
        }
        private async Task RemoveCacheData(BlogPagingRequest? request)
        {
            await _redis.RemoveValue(SystemConstant.BLOG_KEY);
        }

        public async Task<ApiResult<bool>> UpdateView(int blogId)
        {
            var item = await _db.Items.FindAsync(blogId);
            if (item == null) return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");
            item.ViewCount++;
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<bool>();
        }

        public async Task<ApiResult<BlogCombineCreateOrUpdateViewModel>> GetDetailManage(int id)
        {
            var blog = await _db.Blogs
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.ItemId == id);
            if (blog == null) return new ApiErrorResult<BlogCombineCreateOrUpdateViewModel>("Blog not found");
            var data = new BlogCombineCreateOrUpdateViewModel
            {
                Description = blog.Item.Description,
                Detail = blog.Detail,
                Keywords = blog.Item.Keywords,
                Title = blog.Item.Title,
                Thumbnail = blog.Item.Thumbnail,
            };
            return new ApiSuccessResult<BlogCombineCreateOrUpdateViewModel>(data);
        }
    }
}
