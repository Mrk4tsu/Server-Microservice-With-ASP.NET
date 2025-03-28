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
using System.Reflection.Metadata;

namespace FN.Application.Catalog.Blogs.Pattern
{
    public class BlogCreateFacade : BaseService
    {
        public BlogCreateFacade(AppDbContext db, IRedisService dbRedis, IImageService image)
            : base(db, dbRedis, image, SystemConstant.BLOG_KEY)
        {
        }
        public async Task<ApiResult<int>> CreateCombine(BlogCombineCreateOrUpdateRequest request, int userId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                try
                {
                    //Tạo mới Item
                    var newItem = new BaseRequest
                    {
                        Title = request.Title!,
                        Description = request.Description!,
                        Keywords = request.Keywords!,
                        Thumbnail = request.Thumbnail!
                    };
                    var newItemResult = await CreateItemInternal(newItem, userId);
                    if (!newItemResult.Success) return newItemResult;
                    var newBlog = new BlogCreateOrUpdateRequest
                    {
                        Detail = await ProcessContentImages(request.Detail!, $"{newItemResult.Data}/assets")
                    };
                    var newBlogResult = await CreateBlogInternal(newBlog, newItemResult.Data);
                    if (!newBlogResult.Success) return newBlogResult;
                    await transaction.CommitAsync();
                    await RemoveOldCache();
                    return new ApiSuccessResult<int>(newItemResult.Data);
                }
                catch (Exception ex)
                {
                    return new ApiErrorResult<int>("Error: " + ex.Message);
                }
            }
        }
#if POOLING
        //public async Task<ApiResult<int>> CreateCombine(BlogCombineCreateOrUpdateRequest request, int userId)
        //{
        //    var strategy = _db.Database.CreateExecutionStrategy();
        //    return await strategy.ExecuteAsync(async () =>
        //    {
        //        using (var transaction = await _db.Database.BeginTransactionAsync())
        //        {
        //            try
        //            {
        //                // Create new Item
        //                var newItem = new BaseRequest
        //                {
        //                    Title = request.Title!,
        //                    Description = request.Description!,
        //                    Keywords = request.Keywords!,
        //                    Thumbnail = request.Thumbnail!
        //                };
        //                var newItemResult = await CreateItemInternal(newItem, userId);
        //                if (!newItemResult.Success) return newItemResult;

        //                var newBlog = new BlogCreateOrUpdateRequest
        //                {
        //                    Detail = request.Detail!
        //                };
        //                var newBlogResult = await CreateBlogInternal(newBlog, newItemResult.Data);
        //                if (!newBlogResult.Success) return newBlogResult;

        //                var newBlogImage = new BlogImageCreateOrUpdateRequest
        //                {
        //                    ImageDetails = request.ImageDetails
        //                };
        //                var newBlogImageResult = await CreateBlogImageDetail(newBlogImage, newBlogResult.Data);
        //                if (!newBlogImageResult.Success) return newBlogImageResult;

        //                await transaction.CommitAsync();
        //                await RemoveOldCache();
        //                return new ApiSuccessResult<int>(newItemResult.Data);
        //            }
        //            catch (Exception ex)
        //            {
        //                await transaction.RollbackAsync();
        //                return new ApiErrorResult<int>("Error: " + ex.Message);
        //            }
        //        }
        //    });
        //}

#endif

        private async Task<ApiResult<int>> CreateItemInternal(BaseRequest request, int userId)
        {
            var code = StringHelper.GenerateProductCode(request.Title!);
            var newItem = new Item()
            {
                Title = request.Title!,
                Description = request.Description!,
                Keywords = request.Keywords!,
                SeoTitle = request.Title!,
                UserId = userId,
                CreatedDate = Now(),
                ModifiedDate = Now(),
                NormalizedTitle = StringHelper.NormalizeString(request.Title!),
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title!),
                Code = code,
                Thumbnail = "",
            };
            await _db.Items.AddAsync(newItem);
            await _db.SaveChangesAsync();

            string? newThumbnail = await UploadImage(request.Thumbnail!, newItem.Id.ToString(), newItem.Id.ToString());
            if (!string.IsNullOrEmpty(newThumbnail)) newItem.Thumbnail = newThumbnail;

            _db.Items.Update(newItem);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newItem.Id);
        }
        private async Task<ApiResult<int>> CreateBlogInternal(BlogCreateOrUpdateRequest request, int itemId)
        {
            var item = await _db.Items.FindAsync(itemId);
            if (item == null) return new ApiErrorResult<int>("Item not found");

            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");

            var newBlog = new Blog()
            {
                Detail = sanitizer.Sanitize(request.Detail!),
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
