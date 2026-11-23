using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Utilities;

namespace TourSite.Service.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            var key = _configuration["JwtSettings:Key"]
                ?? throw new InvalidOperationException("JwtSettings:Key is missing");
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }

        public int RefreshTokenDays => int.TryParse(
            _configuration["JwtSettings:RefreshTokenDays"], out var days) ? days : 7;

        public async Task<(string Token, DateTime ExpiresAtUtc)> CreateTokenAsync(User user, UserManager<User> userManager)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new("nameidentifier", user.Id),
                new("name", user.FullName ?? user.UserName ?? string.Empty),
                new("email", user.Email ?? string.Empty),
                new("IsActive", user.IsActive ? "1" : "0")
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles.Where(r => !string.IsNullOrWhiteSpace(r)))
                claims.Add(new Claim("role", role));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:AccessTokenMinutes"] ?? "60")),
                claims: claims,
                signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));

            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        public string HashToken(string token)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToHexString(bytes);
        }
    }
}



