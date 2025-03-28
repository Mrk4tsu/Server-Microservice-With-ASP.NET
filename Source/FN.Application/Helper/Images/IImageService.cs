using Microsoft.AspNetCore.Http;

namespace FN.Application.Helper.Images
{
    public interface IImageService
    {
        string GenerateId();
        Task<string> UploadImageRegex(string base64Image, string folder);
        Task<string?> UploadImage(IFormFile file, string publicId, string folderName, string? root);
        Task<List<string>> UploadImages(List<IFormFile> files, string folderName);
        Task<List<string>> UploadImages(List<IFormFile> files, string folderName, string publicId);
        Task<bool> DeleteImage(string publicId);
        Task<bool> DeleteFolderImage(string folderName);
        Task DeleteImageInFolder(string publicId, string folderName);
    }
}
