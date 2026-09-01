
using Microsoft.AspNetCore.Http;

namespace Marketplace.Application.Interfaces.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder, string? subFolder = null);
        Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string folder, string? subFolder = null);
        Task<bool> DeleteFileAsync(string filePath);
        bool ValidateFile(IFormFile file, long maxSize = 5242880, string[]? allowedExtensions = null);
    }
}