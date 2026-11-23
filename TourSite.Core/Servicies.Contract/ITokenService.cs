using TourSite.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace TourSite.Core.Servicies.Contract
{
    public interface ITokenService
    {
        int RefreshTokenDays { get; }
        Task<(string Token, DateTime ExpiresAtUtc)> CreateTokenAsync(User user, UserManager<User> userManager);
        string HashToken(string token);
    }
}
