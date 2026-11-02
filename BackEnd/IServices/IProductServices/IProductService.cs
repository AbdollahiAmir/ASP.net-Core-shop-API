using BackEnd.DTOs.ProductDTOs;
using BackEnd.Models;

namespace BackEnd.IServices.IProductServices
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync(string userId, bool isAdmin);

        Task<Product?> GetByIdAsync(int id, string userId, bool isAdmin);

        Task<Product> CreateAsync(CreateProductDto dto, string userId, bool isAdmin);

        Task<Product?> PatchAsync(int id, PatchProductDto dto, string userId, bool isAdmin);

        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);

        Task<Product?> SetApprovalAsync(int id, bool isApproved);

        Task<(Stream Stream, string ContentType)?> GetProductImageAsync(int id, string userId, bool isAdmin);
    }
}
