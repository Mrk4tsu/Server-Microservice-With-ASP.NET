﻿using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using Microsoft.AspNetCore.Http;

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
        protected async Task<string?> UploadImage(IFormFile thumbnail, string code, string itemId)
        {
            return await _image.UploadImage(thumbnail, code, Folder(itemId.ToString()));
        }
        protected async Task RemoveOldCache()
        {
            switch (ROOT)
            {
                case "product":
                    await _dbRedis.RemoveValue(SystemConstant.CACHE_PRODUCT);
                    break;
                case "category":
                    await _dbRedis.RemoveValue(SystemConstant.CACHE_CATEGORY);
                    break;
                case "blog":
                    await _dbRedis.RemoveValue(SystemConstant.CACHE_BLOG);
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
        protected string Folder(string code)
        {
            return $"{ROOT}/{code}";
        }
    }
}
