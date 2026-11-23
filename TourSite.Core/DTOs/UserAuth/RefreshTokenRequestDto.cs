using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.UserAuth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
