using BackEnd.Models;

namespace BackEnd.IServices.IProductServices
{
    public interface IPublicProductService
    {
        Task<List<Product>> GetApprovedProductsAsync();

        Task<(Stream Stream, string ContentType)?> GetApprovedProductImageAsync(int id);
    }
}
