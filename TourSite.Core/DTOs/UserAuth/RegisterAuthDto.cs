using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.UserAuth
{
    public class RegisterAuthDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }
    }
}
