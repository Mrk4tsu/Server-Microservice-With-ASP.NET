using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Blogs.Pattern
{
    public class BlogUpdateFacade : BaseService
    {
        public BlogUpdateFacade(AppDbContext db, IRedisService dbRedis, IImageService image, string root) 
            : base(db, dbRedis, image, root)
        {
        }
        public async Task<ApiResult<int>> Update(BlogCombineCreateOrUpdateRequest request, int itemId, int blogId, int userId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    var itemUpdate = new BaseRequest
                    {
                        Description = request.Description,
                        Keywords = request.Keywords,
                        Thumbnail = request.Thumbnail,
                        Title = request.Title
                    };
                    var itemResult = await UpdateItemInternal(request, itemId, userId);
                    if (!itemResult.Success) return itemResult;

                    var blogUpdate = new BlogCreateOrUpdateRequest
                    {
                        Detail = request.Detail
                    };
                    var blogResult = await UpdateBlogInternal(blogUpdate, itemResult.Data, blogId);
                    if(!itemResult.Success) return blogResult;                 
                    await RemoveOldCache();
                    await transaction.CommitAsync();
                    return new ApiSuccessResult<int>(itemId);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ApiErrorResult<int>(ex.Message);
                }
            }
        }
        private async Task<ApiResult<int>> UpdateItemInternal(BlogCombineCreateOrUpdateRequest request, int itemId, int userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId);
            if (item == null) return new ApiErrorResult<int>("Không tìm thấy Item");
            if (!string.IsNullOrEmpty(request.Title))
            {
                string code = StringHelper.GenerateProductCode(request.Title);
                item.Title = request.Title;
                item.Code = code;
                item.SeoAlias = StringHelper.GenerateSeoAlias(request.Title);
                item.SeoTitle = request.Title;
                item.NormalizedTitle = StringHelper.NormalizeString(request.Title);
            }
            if (!string.IsNullOrEmpty(request.Description))
                item.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Keywords))
                item.Keywords = request.Keywords;
            if (request.Thumbnail != null)
            {
                string? newThumbnail = await UploadImage(request.Thumbnail, item.Id.ToString(), item.Id.ToString());
                if (!string.IsNullOrEmpty(newThumbnail)) item.Thumbnail = newThumbnail;
            }
            item.ModifiedDate = Now();
            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(item.Id);
        }
        private async Task<ApiResult<int>> UpdateBlogInternal(BlogCreateOrUpdateRequest request, int itemId, int blogId)
        {
            var blog = await _db.Blogs
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == blogId && x.ItemId == itemId);
            if (blog == null) return new ApiErrorResult<int>("Không tìm thấy Blog");

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");
            if (!string.IsNullOrEmpty(request.Detail))
                blog.Detail = sanitizer.Sanitize(request.Detail);

            _db.Blogs.Update(blog);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(blog.Id);
        }
        
    }
}
