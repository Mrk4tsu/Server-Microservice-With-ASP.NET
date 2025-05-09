﻿using Microsoft.AspNetCore.Http;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace FN.Application.Helper.Images
{
    public class ImageService : IImageService
    {
        private const string Root = "public";
        private readonly Cloudinary _cloudinary;
        public ImageService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }
        
        public string GenerateId() => Guid.NewGuid().ToString().Substring(4, 6);

        public async Task<bool> DeleteImage(string publicId)
        {
            var deleteParams = new DeletionParams($"{Root}/{publicId}")
            {
                ResourceType = ResourceType.Image,
            };
            var result = await _cloudinary.DestroyAsync(deleteParams);
            if (result.StatusCode == HttpStatusCode.OK) return true;
            return false;
        }
        public async Task DeleteImageInFolder(string publicId, string folderName)
        {
            var deleteParams = new DeletionParams($"{Root}/{folderName}/{publicId}")
            {
                ResourceType = ResourceType.Image,
                Type = "upload",
            };
            var result = await _cloudinary.DestroyAsync(deleteParams);
        }
        public async Task<bool> DeleteFolderImage(string folderName)
        {
            var result = await _cloudinary.DeleteFolderAsync($"{Root}/{folderName}");
            Console.WriteLine(result.StatusCode.ToString());
            if (result.StatusCode != HttpStatusCode.OK)
                return false;
            return true;
        }
        public async Task<string?> UploadImage(IFormFile file, string publicId, string folderName, string? root = null)
        {
            var folder = $"{Root}/{folderName}";
            if(!string.IsNullOrEmpty(root))
                folder = $"{Root}/{root}/{folderName}";
            if (file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    Transformation = new Transformation().Quality(35).Chain(),
                    Format = "webp",
                    File = new FileDescription(file.FileName, stream),
                    PublicId = publicId,
                    Folder = folder,
                    Overwrite = true
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                    return uploadResult.SecureUrl.AbsoluteUri;
                return null;
            }
            return null;
        }
        public async Task<string> UploadImageRegex(string base64Image, string folderName)
        {
            if (string.IsNullOrEmpty(base64Image)) return string.Empty;
            var folder = $"{Root}/{folderName}";
            var base64Data = Regex.Match(base64Image, @"data:image/(?<type>.+?);base64,(?<data>.+)").Groups["data"].Value;
            var bytes = Convert.FromBase64String(base64Data);
            using var stream = new MemoryStream(bytes);
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = false,
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
        public async Task<List<string>> UploadImages(List<IFormFile> files, string folderName)
        {
            var uploadResults = new List<string>();
            foreach (var image in files)
            {
                var publicId = GenerateId();
                var uploadParam = new ImageUploadParams
                {
                    Transformation = new Transformation().Quality(50).Chain(),
                    File = new FileDescription(image.FileName, image.OpenReadStream()),
                    Folder = $"{Root}/{folderName}",
                    PublicId = publicId,
                    Overwrite = true
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParam);
                uploadResults.Add(uploadResult.SecureUrl.AbsoluteUri);
            }
            return uploadResults;
        }

        public async Task<List<string>> UploadImages(List<IFormFile> files, string folderName, string publicId)
        {
            var uploadResults = new List<string>();
            foreach (var image in files)
            {
                var uploadParam = new ImageUploadParams
                {
                    Transformation = new Transformation().Quality(35).Chain(),
                    File = new FileDescription(image.FileName, image.OpenReadStream()),
                    Folder = $"{Root}/{folderName}",
                    PublicId = publicId,
                    Overwrite = true
                };
                var uploadResult = await _cloudinary.UploadAsync(uploadParam);
                uploadResults.Add(uploadResult.SecureUrl.AbsoluteUri);
            }
            return uploadResults;
        }

        public async Task<string> UploadStream(MemoryStream stream, string folderName)
        {
            var folder = $"{Root}/{folderName}";
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(Guid.NewGuid().ToString(), stream),
                Folder = folder,
                UseFilename = true,
                UniqueFilename = false,
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl.ToString();
        }
    }
}
