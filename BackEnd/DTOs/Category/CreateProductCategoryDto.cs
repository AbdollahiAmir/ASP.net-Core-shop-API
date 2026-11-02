using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Category
{
    public class CreateProductCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public IFormFile? ImgFile { get; set; }
    }
}
