using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace FN.Application.Catalog
{
    public class BaseService
    {
        protected string ROOT;
        protected readonly IRedisService _dbRedis;
        protected readonly IImageService _image;
        protected readonly AppDbContext _db;
        public BaseService(AppDbContext db, IRedisService dbRedis, IImageService image, string root)
        {
            _db = db;
            _dbRedis = dbRedis;
            _image = image;
            ROOT = root;
        }
        protected async Task<string> ProcessContentImages(string content, string folder)
        {
            var regex = new Regex(@"<img[^>]*src=""data:image/(?<type>[a-z]+);base64,(?<data>[^""]+)""[^>]*>", RegexOptions.IgnoreCase);
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                var imageUrl = await _image.UploadImageRegex(match.Value, Folder(folder));
                content = content.Replace(match.Value, $"<img src=\"{imageUrl}\" />");
            }

            return content;
        }
        protected async Task<string?> UploadImage(IFormFile thumbnail, string publicId, string itemId)
        {
            return await _image.UploadImage(thumbnail, publicId, Folder(itemId.ToString()), null);
        }
        protected async Task RemoveOldCache()
        {
            switch (ROOT)
            {
                case "product":
                    await _dbRedis.RemoveValue(SystemConstant.PRODUCT_KEY);
                    break;
                case "category":
                    await _dbRedis.RemoveValue(SystemConstant.CATEGORY_KEY);
                    break;
                case "blog":
                    await _dbRedis.RemoveValue(SystemConstant.BLOG_KEY);
                    break;
                default:
                    break;
            }
        }
        protected DateTime Now()
        {
            DateTime timeNow = new TimeHelper.Builder()
                .SetTimestamp(DateTime.UtcNow)
                .SetTimeZone("SE Asia Standard Time")
                .SetRemoveTick(true).Build();
            return timeNow;
        }
        protected string Folder(string folderChild)
        {
            return $"{ROOT}/{folderChild}";
        }
    }
}
