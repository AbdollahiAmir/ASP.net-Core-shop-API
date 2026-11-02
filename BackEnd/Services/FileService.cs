using BackEnd.IServices;
using BackEnd.Models;

namespace BackEnd.Services
{
    public class FileService : IFileService
    {
        private readonly string _productsFolder;
        private readonly string _categoriesFolder;
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
            var uploadsRoot = Path.Combine(_env.ContentRootPath, "PrivateUploads");

            _productsFolder = Path.Combine(uploadsRoot, "Products");
            _categoriesFolder = Path.Combine(uploadsRoot, "Categories");
        }
        private static async Task<string> SaveImageAsync(IFormFile image, string folder, string folderName)
        {
            if (image is null || image.Length == 0)
            {
                throw new InvalidOperationException("تصویر خالی است");
            }

            if (image.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("حجم تصویر نباید بیشتر از 5 مگابایت باشد");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("فرمت تصویر پشتیبانی نمیشود");
            }

            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write);

            await image.CopyToAsync(stream);

            return $"/Uploads/{folderName}/{fileName}";
        }

        private static Task<(Stream Stream, string ContentType)?> OpenImageFromFolderAsync(string? imagePath, string folder)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return Task.FromResult<(Stream Stream, string ContentType)?>(null);
            }

            var fileName = Path.GetFileName(imagePath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Task.FromResult<(Stream Stream, string ContentType)?>(null);
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => null
            };
            if (contentType is null)
            {
                return Task.FromResult<(Stream Stream, string ContentType)?>(null);
            }

            var filePath = Path.Combine(folder, fileName);
            if (!File.Exists(filePath))
            {
                return Task.FromResult<(Stream Stream, string ContentType)?>(null);
            }

            Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            return Task.FromResult<(Stream Stream, string ContentType)?>((stream, contentType));
        }

        private static Task DeleteImageFromFolderAsync(string? imagePath, string folder)
        {
            if(string.IsNullOrWhiteSpace(imagePath))
            {
                return Task.CompletedTask;
            }

            var fileName = Path.GetFileName(imagePath);
            if(string.IsNullOrWhiteSpace(fileName))
            {
                return Task.CompletedTask;
            }

            var filePath = Path.Combine(folder, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        // Product

        public Task<string> SaveProductImageAsync(IFormFile image)
        {
            return SaveImageAsync(image, _productsFolder, "Products");
        }

        public Task DeleteProductImageAsync(string? imagePath)
        {
            return DeleteImageFromFolderAsync(imagePath, _productsFolder);
        }

        public Task<(Stream Stream, string ContentType)?> OpenProductImageAsync(string? imagePath)
        {
            return OpenImageFromFolderAsync(imagePath, _productsFolder);
        }

        // Product Category

        public Task<string> SaveCategoryImageAsync(IFormFile image)
        {
            return SaveImageAsync(image, _categoriesFolder, "Categories");
        }

        public Task DeleteCategoryImageAsync(string? imagePath)
        {
            return DeleteImageFromFolderAsync(imagePath, _categoriesFolder);
        }

        public Task<(Stream Stream, string ContentType)?> OpenCategoryImageAsync(string? imagePath)
        {
            return OpenImageFromFolderAsync(imagePath, _categoriesFolder);
        }
    }
}
