using BackEnd.DTOs.Category;
using BackEnd.Models;

namespace BackEnd.IServices
{
    public interface IProductCategoryService
    {
        Task<List<ProductCategory>> GetAllAsync();
        Task<ProductCategory?> GetByIdAsync(int id);
        Task<ProductCategory> CreateAsync(CreateProductCategoryDto dto);
        Task<ProductCategory?> PatchAsync(int id, PatchProductCategoryDto dto);
        Task<bool> DeleteAsync(int id);
        Task<(Stream Stream, string ContentType)?> GetCategoryImageAsync(int id);
    }
}
