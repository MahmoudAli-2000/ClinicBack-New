using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using TourSite.APIs.Errors;
using TourSite.Core.DTOs.UserAuth;
using TourSite.Core.Servicies.Contract;
using TourSite.Service.Services.Auth;

namespace TourSite.APIs.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _environment;

        public AuthController(IAuthService authService, ITokenService tokenService, IWebHostEnvironment environment)
        {
            _authService = authService;
            _tokenService = tokenService;
            _environment = environment;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserAuthDto>> Register([FromBody] RegisterAuthDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.RegisterAsync(dto, ipAddress);
            SetAuthCookie(result.AccessToken);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAuthDto>> Login([FromBody] LoginAuthDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.LoginAsync(dto, ipAddress);

            SetAuthCookie(result.AccessToken);

            return Ok(result);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<UserAuthDto>> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.RefreshAsync(dto.RefreshToken, ipAddress);
            return Ok(result);
        }

        [HttpPost("revoke")]
        public async Task<ActionResult> Revoke([FromBody] RefreshTokenRequestDto dto)
        {
            await _authService.RevokeRefreshTokenAsync(dto.RefreshToken);
            Response.Cookies.Delete("jwt", GetAuthCookieOptions());
            return Ok(new { message = "Refresh token revoked." });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt", GetAuthCookieOptions());
            return Ok(new { message = "Logged out successfully." });
        }

        private void SetAuthCookie(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("The authentication service returned an empty access token.");

            Response.Cookies.Append("jwt", token, GetAuthCookieOptions());
        }

        private CookieOptions GetAuthCookieOptions() => new()
        {
            HttpOnly = true,
            Secure = !_environment.IsDevelopment(),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _authService.ForgotPasswordAsync(dto.Email);
            return Ok(new { message = "If the email exists, a reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _authService.ResetPasswordAsync(dto);
            return Ok(new { message = "Password has been reset." });
        }

        [HttpPost("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new APIErrerResponse(401, "Authentication required."));

            await _authService.ChangePasswordAsync(userId, dto);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            await _authService.ConfirmEmailAsync(userId, token);
            return Ok(new { message = "Email confirmed." });
        }

        [HttpPost("resend-confirmation")]
        public async Task<ActionResult> ResendConfirmation([FromQuery] string email)
        {
            await _authService.ResendConfirmationAsync(email);
            return Ok(new { message = "If the email exists and is unconfirmed, a confirmation link has been sent." });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<CurrentUserDto>> GetCurrentUser()
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new APIErrerResponse(401, "Authentication required."));

            var result = await _authService.GetCurrentUserAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<ActionResult<CurrentUserDto>> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new APIErrerResponse(401, "Authentication required."));

            var result = await _authService.UpdateProfileAsync(userId, dto);
            return Ok(result);
        }
    }
}

