using System.ComponentModel.DataAnnotations.Schema;

namespace BackEnd.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        public string? ImgPath { get; set; }

        public string? SellerId { get; set; }

        public AppUser? Seller { get; set; }

        public bool IsApproved { get; set; } = false;

        [NotMapped]
        public string? SellerUserName { get; set; }

        public int? CategoryId { get; set; }

        public ProductCategory? Category { get; set; } = null!;
    }
}
