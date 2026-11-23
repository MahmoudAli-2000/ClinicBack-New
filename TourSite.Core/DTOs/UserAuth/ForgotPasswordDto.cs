using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.UserAuth
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
