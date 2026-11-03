using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Order.Admin
{
    public class AdminSellerDecisionDto
    {
        [Required]
        public bool? Approve { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }
    }
}
