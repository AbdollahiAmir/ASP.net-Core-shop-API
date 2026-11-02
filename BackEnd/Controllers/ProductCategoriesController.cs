using BackEnd.DTOs.Category;
using BackEnd.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly IProductCategoryService _service;

        public ProductCategoriesController(IProductCategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProductCategories()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCategoryById(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if(category is  null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpGet("{id}/image")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryImage(int id)
        {
            var image = await _service.GetCategoryImageAsync(id);
            if(image is null)
            {
                return NotFound();
            }

            return File(image.Value.Stream, image.Value.ContentType);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductCategory([FromForm] CreateProductCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("نام دسته بندی الزامی است");
            }

            var category = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetProductCategoryById), new { id = category.Id }, category);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditProductCategory(int id, [FromForm] PatchProductCategoryDto dto)
        {
            if (dto.Name is not null && string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("نام دسته بندی نمیتواند خالی باشد");
            }

            if (dto.Name is null && dto.ImgFile is null)
            {
                return BadRequest("باید نام یا تصویر را تغییر دهید");
            }

            var category = await _service.PatchAsync(id, dto);
            if(category is null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if(!deleted)
            {
                NoContent();
            }

            return NoContent();
        }
    }
}
