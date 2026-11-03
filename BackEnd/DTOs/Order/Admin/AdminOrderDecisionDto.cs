using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Order.Admin
{
    public class AdminOrderDecisionDto
    {
        [Required]
        [StringLength(500, MinimumLength = 3)]
        public string Reason { get; set; } = string.Empty;
    }
}
