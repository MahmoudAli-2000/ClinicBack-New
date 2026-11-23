using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Staff
{
    public class StaffUpdateDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
        [MaxLength(30)]
        public string? Phone { get; set; }
        [EmailAddress, MaxLength(254)]
        public string? Email { get; set; }
        public int? PrimaryClinicId { get; set; }
        [MaxLength(120)]
        public string? JobTitle { get; set; }
        public StaffStatus Status { get; set; }
        public DateTime? HireDate { get; set; }
        public List<string>? Roles { get; set; }
    }
}
