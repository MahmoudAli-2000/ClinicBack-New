using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.UserAuth
{
    public class UpdateProfileDto
    {
        [Required, MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
