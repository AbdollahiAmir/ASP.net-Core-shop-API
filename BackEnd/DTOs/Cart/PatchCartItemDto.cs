using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Cart
{
    public class PatchCartItemDto
    {
        [Range(0, 1000)]
        public int Quantity { get; set; }
    }
}
