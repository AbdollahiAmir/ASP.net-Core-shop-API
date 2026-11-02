using System.ComponentModel.DataAnnotations;

namespace BackEnd.DTOs.Users
{
    public class EditUserDto
    {
        [Required]
        [StringLength(250, MinimumLength = 3)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;
    }
}
