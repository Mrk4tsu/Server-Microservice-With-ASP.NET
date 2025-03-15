using FN.Application.Catalog.Blogs.Pattern;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
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
    }
}
