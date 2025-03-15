using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;
using Ganss.Xss;

namespace FN.Application.Catalog.Blogs.Pattern
{
    public class BlogCreateFacade : BaseService
    {
        public BlogCreateFacade(AppDbContext db, IRedisService dbRedis, IImageService image)
            : base(db, dbRedis, image, "blog")
        {
        }
        public async Task<ApiResult<int>> CreateCombine(BlogCombineCreateRequest request, int userId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    //Tạo mới Item
                    var newItem = new BaseCreateRequest
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Keywords = request.Keywords,
                        Thumbnail = request.Thumbnail
                    };
                    var newItemResult = await CreateItemInternal(newItem, userId);
                    if (!newItemResult.Success) return newItemResult;
                    var newBlog = new BlogCreateRequest
                    {
                        Detail = request.Detail
                    };
                    var newBlogResult = await CreateBlogInternal(newBlog, newItemResult.Data);
                    if (!newBlogResult.Success) return newBlogResult;

                    await transaction.CommitAsync();
                    await RemoveOldCache();
                    return new ApiSuccessResult<int>(newBlogResult.Data);
                }
                catch (Exception ex)
                {
                    return new ApiErrorResult<int>("Error: " + ex.Message);
                }
            }
        }
        private async Task<ApiResult<int>> CreateItemInternal(BaseCreateRequest request, int userId)
        {
            var code = StringHelper.GenerateProductCode(request.Title!);
            var newItem = new Item()
            {
                Title = request.Title!,
                Description = request.Description!,
                Keywords = request.Keywords!,
                SeoTitle = request.Title!,
                UserId = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                NormalizedTitle = StringHelper.NormalizeString(request.Title!),
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title!),
                Code = code,
                Thumbnail = "",
            };
            await _db.Items.AddAsync(newItem);
            await _db.SaveChangesAsync();

            string? newThumbnail = await UploadThumbnail(request.Thumbnail, newItem.Code, Folder(newItem.Id.ToString()));
            if (!string.IsNullOrEmpty(newThumbnail)) newItem.Thumbnail = newThumbnail;

            _db.Items.Update(newItem);
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<int>(newItem.Id);
        } 
        private async Task<ApiResult<int>> CreateBlogInternal(BlogCreateRequest request, int itemId)
        {
            var item = await _db.Items.FindAsync(itemId);
            if (item == null) return new ApiErrorResult<int>("Item not found");

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");

            var newBlog = new Blog()
            {
                Detail = sanitizer.Sanitize(request.Detail),
                ItemId = itemId,
                DislikeCount = 0,
                LikeCount = 0,
            };

            _db.Blogs.Add(newBlog);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newBlog.Id);
        }
    }
}
