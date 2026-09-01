

using Marketplace.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Marketplace.Application.Services
{
    public class FileUploadService : IFileUploadService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<FileUploadService> _logger;

        public FileUploadService(
            IConfiguration configuration,
            ILogger<FileUploadService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return false;

                var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath.TrimStart('/'));

                if(File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    return true;
                }

               
                return false;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return false;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string folder, string? subFolder = null)
        {
            try
            {
                if (!ValidateFile(file))
                    throw new ArgumentException("Invalid file");

                var uploadFolder = Path.Combine("wwwroot", "uploads", folder);

                if(!string.IsNullOrEmpty(subFolder))
                    uploadFolder = Path.Combine(uploadFolder, subFolder);

                Directory.CreateDirectory(uploadFolder);

                var extension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using(var stream = new FileStream(filePath,FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return filePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                throw;
            }
        }

        public async Task<List<string>> UploadMultipleFilesAsync(List<IFormFile> files, string folder, string? subFolder = null)
        {
            var uploadedFiles = new List<string>();
            foreach (var file in files)
            {
                var filePath = await UploadFileAsync(file, folder, subFolder);
                uploadedFiles.Add(filePath);
            }

            return uploadedFiles;
        }

        public bool ValidateFile(IFormFile file, long maxSize = 5242880, string[]? allowedExtensions = null)
        {
            if(file == null || file.Length == 0) return false;


            if(file.Length >  maxSize) return false;    

            if(allowedExtensions == null)
                allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            var extension = Path.GetExtension(file.FileName).ToLower();

            if(!allowedExtensions.Contains(extension)) return false;

            return true;


        }
    }
}
