﻿using FN.Application.Helper.Images;
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

                    var newBlogImage = new BlogImageCreateRequest
                    {
                        ImageDetails = request.ImageDetails
                    };
                    var newBlogImageResult = await CreateBlogImageDetail(newBlogImage, newBlogResult.Data);
                    if (!newBlogImageResult.Success) return newBlogImageResult;

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

            string? newThumbnail = await UploadImage(request.Thumbnail, newItem.Code, newItem.Id.ToString());
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
        private async Task<ApiResult<int>> CreateBlogImageDetail(BlogImageCreateRequest request, int blogId)
        {
            var blog = await _db.Blogs.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == blogId);
            if (blog == null) return new ApiErrorResult<int>("Blog not found");
            if (blog.BlogImages == null) blog.BlogImages = new List<BlogImage>();

            //Nếu không có ảnh thì bỏ qua việc upload ảnh
            if (request.ImageDetails == null || !request.ImageDetails.Any()) return new ApiSuccessResult<int>(blogId); ;

            foreach (var imageFile in request.ImageDetails)
            {
                var publicId = _image.GenerateId();
                var newImage = await UploadImage(imageFile, publicId, $"{blog.ItemId.ToString()}/assets");
                if (newImage == null) return new ApiErrorResult<int>("Upload image failed");
                var newBlogImage = new BlogImage()
                {
                    BlogId = blogId,
                    ImageUrl = newImage,
                    Caption = blog.Item.Title,
                    PublicId = publicId
                };
                _db.BlogsImages.Add(newBlogImage);
            }
            var result = await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(result);
        }
    }
}
