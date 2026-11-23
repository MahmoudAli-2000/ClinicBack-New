using TourSite.Core.DTOs.UserAuth;

namespace TourSite.Core.Servicies.Contract
{
    public interface IAuthService
    {
        Task<UserAuthDto> RegisterAsync(RegisterAuthDto dto, string? ipAddress, CancellationToken cancellationToken = default);
        Task<UserAuthDto> LoginAsync(LoginAuthDto dto, string? ipAddress, CancellationToken cancellationToken = default);
        Task<UserAuthDto> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
        Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
        Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(ResetPasswordDto dto, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(string userId, ChangePasswordDto dto, CancellationToken cancellationToken = default);
        Task ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
        Task ResendConfirmationAsync(string email, CancellationToken cancellationToken = default);
        Task<CurrentUserDto> GetCurrentUserAsync(string userId, CancellationToken cancellationToken = default);
        Task<CurrentUserDto> UpdateProfileAsync(string userId, UpdateProfileDto dto, CancellationToken cancellationToken = default);
    }
}
