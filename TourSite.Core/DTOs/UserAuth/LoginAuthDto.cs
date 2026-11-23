using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.UserAuth
{
    public class LoginAuthDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
