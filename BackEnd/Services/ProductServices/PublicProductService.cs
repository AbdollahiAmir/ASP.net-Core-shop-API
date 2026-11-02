using BackEnd.Data;
using BackEnd.IServices;
using BackEnd.IServices.IProductServices;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Services.ProductServices
{
    public class PublicProductService : IPublicProductService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public PublicProductService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }
        public async Task<List<Product>> GetApprovedProductsAsync()
        {
            return await _context.Products.Where(p => p.IsApproved).Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                ImgPath = p.ImgPath,
                CategoryId = p.CategoryId,
                Category = p.Category == null ? null : new ProductCategory
                        {
                            Id = p.Category.Id,
                            Name = p.Category.Name
                        }
            }).ToListAsync();
        }

        public async Task<(Stream Stream, string ContentType)?> GetApprovedProductImageAsync(int id)
        {
            var product = await _context.Products.Where(p => p.Id == id && p.IsApproved).Select(p=> new Product
            {
                Id = p.Id,
                ImgPath= p.ImgPath,
            }).FirstOrDefaultAsync();
            if(product is null)
            {
                return null;
            }

            return await _fileService.OpenProductImageAsync(product.ImgPath);
        }
    }
}
