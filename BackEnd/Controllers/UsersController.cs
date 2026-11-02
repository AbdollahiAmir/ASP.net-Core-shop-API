using BackEnd.DTOs.Users;
using BackEnd.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _service;

        public UsersController(IUsersService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "کاربر یافت نشد." });
            }

            return Ok(user);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> EditUser(string id, EditUserDto dto)
        {
            var result = await _service.EditAsync(id, dto);
            if (!result.Succeeded)
            {
                if (result.Errors.Contains("کاربر یافت نشد."))
                {
                    return NotFound(new { message = "کاربر یافت نشد" });
                }
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "ویرایش کاربر موفقیت آمیز بود" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.DeleteAsync(id, currentUserId);
            if (!result.Succeeded)
            {
                if (result.Errors.Contains("کاربر یافت نشد"))
                {
                    return NotFound(new { errors = result.Errors });
                }

                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "کاربر با موفقیت حذف شد." });
        }
    }
}
