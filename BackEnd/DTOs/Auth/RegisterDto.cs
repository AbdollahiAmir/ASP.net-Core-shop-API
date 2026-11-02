using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Auth
{
    public class RegisterDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ComparePassword { get; set; } = string.Empty;
    }
}
