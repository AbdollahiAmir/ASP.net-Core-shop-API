namespace BackEnd.DTOs.Category
{
    public class PatchProductCategoryDto
    {
        public string? Name { get; set; }

        public IFormFile? ImgFile { get; set; }
    }
}
