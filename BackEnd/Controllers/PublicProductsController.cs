using BackEnd.IServices.IProductServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicProductsController : ControllerBase
    {
        private readonly IPublicProductService _service;

        public PublicProductsController(IPublicProductService service)
        {
            _service = service;
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetPublicProducts()
        {
            var products = await _service.GetApprovedProductsAsync();

            return Ok(products);
        }

        [HttpGet("products/{id}/image")]
        public async Task<IActionResult> GetPublicProductImage(int id)
        {
            var image = await _service.GetApprovedProductImageAsync(id);
            if(image is null)
            {
                return NotFound();
            }

            return File(image.Value.Stream, image.Value.ContentType);
        }
    }
}
