namespace BackEnd.DTOs.ProductDTOs
{
    public class CreateProductDto
    {
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public IFormFile? ImgFile { get; set; }
    }
}
