using BackEnd.DTOs.Auth;
using BackEnd.IServices;
using BackEnd.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _service.RegisterAsync(dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "ثبت نام با موفقیت انجام شد" });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _service.LoginAsync(dto);
            if (!result.IsSuccess)
            {
                return Unauthorized(new { errors = result.Errors });
            }

            if (string.IsNullOrEmpty(result.Token) || result.ExpiresAt is null)
            {
                return Unauthorized(new { message = "احراز هویت با شکست مواجه شد." });
            }

            Response.Cookies.Append("access_token", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = new DateTimeOffset(result.ExpiresAt.Value)
                });

            return Ok(new
            {
                message = "ورود با موفقیت انجام شد.",
                expiresAt = result.ExpiresAt
            });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/"
                });

            return Ok(new { message = "خروج با موفقیت انجام شد" });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.Identity?.Name;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName))
            {
                return Unauthorized();
            }

            var roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();

            return Ok(new
            {
                userId,
                userName,
                roles
            });
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var result = await _service.ChangePasswordAsync(userId, dto);

            if (!result.IsSuccess)
            {
                return BadRequest(new { errors = result.Errors });
            }

            return Ok(new { message = "تغییر رمز با موفقیت انجام شد" });
        }
    }
}
