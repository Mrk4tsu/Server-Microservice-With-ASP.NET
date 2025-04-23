using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

namespace FN.Application.Catalog
{
    public class BaseService
    {
        protected string ROOT;
        protected readonly IRedisService _dbRedis;
        protected readonly IImageService _image;
        protected readonly AppDbContext _db;
        protected readonly IHttpClientFactory _httpClientFactory;
        public BaseService(AppDbContext db,
            IHttpClientFactory httpClientFactory,
            IRedisService dbRedis, IImageService image, string root)
        {
            _db = db;
            _dbRedis = dbRedis;
            _image = image;
            _httpClientFactory = httpClientFactory;
            ROOT = root;
        }

        protected string ProcessSantizer(string content)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");
            sanitizer.AllowedCssProperties.Clear();
            sanitizer.AllowedCssProperties.Add("font-size");
            sanitizer.AllowedCssProperties.Add("font-style");
            sanitizer.AllowedCssProperties.Add("line-height");

            return sanitizer.Sanitize(content);
        }
        protected async Task<string> HandleCoverImage(IFormFile image, string itemId, string publicId)
        {
            var backgroundUrl = "https://res.cloudinary.com/dje3seaqj/image/upload/v1745248537/cover_bp5msi.png";
            using var avatarStream = image.OpenReadStream();
            using Image<Rgba32> avatarImage = await Image.LoadAsync<Rgba32>(avatarStream);

            // Load background từ URL
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync(backgroundUrl);
            if (!response.IsSuccessStatusCode)
                return string.Empty;

            await using var bgStream = await response.Content.ReadAsStreamAsync();
            using Image<Rgba32> backgroundImage = await Image.LoadAsync<Rgba32>(bgStream);
            avatarImage.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new SixLabors.ImageSharp.Size(350, 350), // chỉnh tùy vị trí và kích cỡ bạn muốn
                Mode = ResizeMode.Stretch,
            }));
            // Ghép avatar vào background (vị trí x=680, y=115 là ví dụ)
            backgroundImage.Mutate(x => x.DrawImage(avatarImage, new Point(700, 140), 1f));

            // Xuất ảnh kết quả
            var resultStream = new MemoryStream();
            await backgroundImage.SaveAsPngAsync(resultStream);
            resultStream.Seek(0, SeekOrigin.Begin);

            //public/user
            var seoImage = await _image.UploadStream(resultStream, Folder(itemId.ToString()), publicId);
            
            return seoImage;
        }
        protected async Task<string> ProcessContentImages(string content, int itemId)
        {
            var regex = new Regex(@"<img[^>]*src\s*=\s*[""']?data:image/(?<type>[a-z]+);base64,(?<data>[^""']*)[""']?[^>]*>",
                   RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(1500));
            var matches = regex.Matches(content);

            foreach (Match match in matches)
            {
                var fullMatch = match.Value;
                var base64Data = match.Groups["data"].Value;
                var imageType = match.Groups["type"].Value;
                var sanitizedData = base64Data
                    .Trim()
                    .Replace(" ", "+")
                    .Replace("\n", "")
                    .Replace("\r", "");

                // Thêm padding nếu cần
                if (sanitizedData.Length % 4 > 0)
                    sanitizedData = sanitizedData.PadRight(sanitizedData.Length + (4 - sanitizedData.Length % 4), '=');

                var imageBytes = Convert.FromBase64String(sanitizedData);
                using var stream = new MemoryStream(imageBytes);


                var imageUrl = await _image.UploadStream(stream, $"{Folder(itemId.ToString())}/assets");
                content = content.Replace(fullMatch,
                match.Value.Replace(
                    $"data:image/{imageType};base64,{base64Data}",
                    imageUrl
                ));
            }

            return content;
        }
        protected async Task<string?> UploadImage(IFormFile thumbnail, string publicId, string itemId)
        {
            return await _image.UploadImage(thumbnail, publicId, Folder(itemId.ToString()), null);
        }
        protected async Task RemoveOldCache(string? param = null)
        {
            if (param != null)
            {
                await _dbRedis.RemoveValue(param);
            }
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
            await _dbRedis.RemoveCache(SystemConstant.PRODUCT_KEY + "_recommend");
            await _dbRedis.RemoveCache(SystemConstant.PRODUCT_KEY + "_new");
            await _dbRedis.RemoveCache(SystemConstant.PRODUCT_KEY + "_feature");
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
