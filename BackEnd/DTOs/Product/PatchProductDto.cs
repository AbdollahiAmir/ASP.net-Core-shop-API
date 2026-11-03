namespace BackEnd.DTOs.ProductDTOs
{
    public class PatchProductDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public IFormFile? ImgFile { get; set; }
    }
}
