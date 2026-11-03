using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Order.Seller
{
    public class SellerFulfillmentDecisionDto
    {
        [Required]
        public bool? CanSupply { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}
