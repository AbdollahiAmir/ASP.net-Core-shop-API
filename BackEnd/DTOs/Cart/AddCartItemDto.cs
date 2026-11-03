using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Cart
{
    public class AddCartItemDto
    {
        [Range(0, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(0, 100)]
        public int Quantity { get; set; } = 1;
    }
}
