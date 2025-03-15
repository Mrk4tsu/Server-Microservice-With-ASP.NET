using FN.Application.Helper.Images;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Helper.API;

namespace FN.Application.Catalog.Blogs
{
    public class BlogService : IBlogService
    {
        private string ROOT = "blog";
        private readonly AppDbContext _db;
        private readonly IImageService _image;
        public BlogService(AppDbContext db, IImageService image)
        {
            _db = db;
            _image = image;
        }
        string Folder(string code)
        {
            return $"{ROOT}/{code}";
        }

        public async Task<ApiResult<int>> CreateItem(BaseCreateRequest request, int uid)
        {
            var code = StringHelper.GenerateProductCode(request.Title!);
            var newItem = new Item()
            {
                Title = request.Title!,
                Description = request.Description!,
                Keywords = request.Keywords!,
                SeoTitle = request.Title!,
                UserId = uid,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                NormalizedTitle = StringHelper.NormalizeString(request.Title!),
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title!),
                Code = code,
                Thumbnail = "",
            };
            await _db.Items.AddAsync(newItem);
            await _db.SaveChangesAsync();

            newItem.Thumbnail = await _image.UploadImage(request.Thumbnail, newItem.Code, Folder(newItem.Id.ToString())) ?? "";
            _db.Items.Update(newItem);
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<int>(newItem.Id);
        }

        public async Task<ApiResult<int>> CreateBlog(BlogCreateRequest request, int itemId)
        {
            var item = await _db.Items.FindAsync(itemId);
            if (item == null) return new ApiErrorResult<int>("Item not found");

            var newBlog = new Blog()
            {
                Detail = request.Detail,
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
