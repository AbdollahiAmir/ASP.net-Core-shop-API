using BackEnd.DTOs.ProductDTOs;
using BackEnd.IServices.IProductServices;
using BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Seller")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        private string? CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
        private bool IsAdmin => User.IsInRole("Admin");

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var products = await _service.GetAllAsync(userId, IsAdmin);

            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var product = await _service.GetByIdAsync(id, userId, IsAdmin);
            if(product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("نام محصول الزامی است");
            }

            if (dto.Price <= 0)
            {
                return BadRequest("قیمت باید از 0 بیشتر باشد");
            }

            if (dto.ImgFile is not null && (dto.ImgFile.Length == 0 || dto.ImgFile.Length > 5 * 1024 * 1024))
            {
                return BadRequest("حجم عکس نباید بیشتر از 5 مگابایت باشد");
            }

            Product product;

            try
            {
                product = await _service.CreateAsync(dto, userId, IsAdmin);
            }
            catch(ArgumentException)
            {
                return BadRequest("دسته بندی نامعتبر");
            }

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPatch("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditProduct(int id, [FromForm] PatchProductDto dto)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("نام محصول نمیتواند خالی باشد");
            }

            if (dto.Price.HasValue && dto.Price.Value <= 0)
            {
                return BadRequest("فیمت محصول باید بیشتر از 0 باشد");
            }

            if (dto.ImgFile is not null && (dto.ImgFile.Length == 0 || dto.ImgFile.Length > 5 * 1024 * 1024))
            {
                return BadRequest("حجم عکس باید از 5 مگابایت کمتر باشد");
            }

            Product? product;

            try
            {
                product = await _service.PatchAsync(id, dto, userId, IsAdmin);
            }
            catch (ArgumentException)
            {
                return BadRequest("دسته بندی نامعتبر");
            }

            if (product is null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var deleted = await _service.DeleteAsync(id, userId, IsAdmin);
            if(!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPatch("{id}/approval")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetApproval(int id, [FromQuery] bool isApproved)
        {
            try
            {
                var product = await _service.SetApprovalAsync(id, isApproved);

                if (product is null)
                    return NotFound();

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var image = await _service.GetProductImageAsync(id, userId, IsAdmin);
            if(image is null)
            {
                return NotFound();
            }

            return File(image.Value.Stream, image.Value.ContentType);
        }
    }
}
