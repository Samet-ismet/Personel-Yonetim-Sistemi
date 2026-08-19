using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kipas.Personel.API.DTOs;
using Kipas.Personel.API.Helpers;
using Kipas.Personel.API.Interfaces;
using Microsoft.AspNetCore.RateLimiting;


namespace Kipas.Personel.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        

        [AllowAnonymous]
        [HttpPost("login")]
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> Login(
            LoginDto dto)
        {
            var result =
                await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Kullanıcı adı veya şifre hatalı."
                    });
            }

            return Ok(
                new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Giriş başarılı.",
                    Data = result
                });
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        [EnableRateLimiting("AuthLimit")]
        public async Task<IActionResult> Refresh(
            RefreshTokenRequestDto dto)
        {
            var result =
                await _authService.RefreshAsync(
                    dto.RefreshToken);

            if (result == null)
            {
                return Unauthorized(
                    new ApiResponse<object?>
                    {
                        Success = false,
                        Message =
                            "Refresh token geçersiz, iptal edilmiş veya süresi dolmuş."
                    });
            }

            return Ok(
                new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message =
                        "Token başarıyla yenilendi.",
                    Data = result
                });
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout(
            RefreshTokenRequestDto dto)
        {
            await _authService.LogoutAsync(
                dto.RefreshToken);

            return Ok(
                new ApiResponse<object?>
                {
                    Success = true,
                    Message =
                        "Oturum sonlandırma işlemi tamamlandı."
                });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var username =
                User.Identity?.Name;

            var role =
                User.FindFirst(
                    ClaimTypes.Role)?.Value;

            var userId =
                User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

            return Ok(
                new ApiResponse<object>
                {
                    Success = true,
                    Message =
                        "Token başarıyla doğrulandı.",

                    Data = new
                    {
                        UserId = userId,
                        Username = username,
                        Role = role
                    }
                });
        }
    }
}