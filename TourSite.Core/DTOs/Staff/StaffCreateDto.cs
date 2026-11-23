using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Staff
{
    public class StaffCreateDto
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
        public DateTime? HireDate { get; set; }
        public bool CreateAccount { get; set; }
        public string? Password { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
