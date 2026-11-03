using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Order.Customer
{
    public class CreateOrderDto
    {
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string DeliveryAddress { get; set; } = string.Empty;
    }
}
