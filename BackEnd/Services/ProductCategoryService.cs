using BackEnd.Data;
using BackEnd.DTOs.Category;
using BackEnd.IServices;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services
{
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ProductCategoryService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<List<ProductCategory>> GetAllAsync()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<ProductCategory?> GetByIdAsync(int id)
        {
            return await _context.ProductCategories.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ProductCategory> CreateAsync(CreateProductCategoryDto dto)
        {
            var category = new ProductCategory
            {
                Name = dto.Name.Trim()
            };

            if (dto.ImgFile is { Length: > 0 })
            {
                category.ImgPath = await _fileService.SaveCategoryImageAsync(dto.ImgFile);
            }

            _context.ProductCategories.Add(category);

            try
            {
                await _context.SaveChangesAsync();
                return category;
            }
            catch
            {
                await _fileService.DeleteCategoryImageAsync(category.ImgPath);
                throw;
            }
        }

        public async Task<ProductCategory?> PatchAsync(int id, PatchProductCategoryDto dto)
        {
            var category = await GetByIdAsync(id);
            if(category is null)
            {
                return null;
            }

            if (dto.Name is not null)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    throw new ArgumentException("نام دسته بندی الزامی است");
                }

                category.Name = dto.Name.Trim();
            }

            var oldImagePath = category.ImgPath;
            string? newImagePath = null;

            if (dto.ImgFile is { Length: > 0 })
            {
                newImagePath = await _fileService.SaveCategoryImageAsync(dto.ImgFile);

                category.ImgPath = newImagePath;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (newImagePath is not null)
                {
                    await _fileService.DeleteCategoryImageAsync(newImagePath);
                }

                throw;
            }

            if (newImagePath is not null)
            {
                await _fileService.DeleteCategoryImageAsync(oldImagePath);
            }

            return category;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category is null)
            {
                return false;
            }

            var imagePath = category.ImgPath;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            var products = await _context.Products.Where(p => p.CategoryId == id).ToListAsync();

            foreach (var product in products)
            {
                product.CategoryId = null;
                product.IsApproved = false;
            }

            _context.ProductCategories.Remove(category);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            await _fileService.DeleteCategoryImageAsync(imagePath);

            return true;
        }

        public async Task<(Stream Stream, string ContentType)?> GetCategoryImageAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if(category is null)
            {
                return null;
            }

            return await _fileService.OpenCategoryImageAsync(category.ImgPath);
        }
    }
}
