using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Patients
{
    public class PatientCreateDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        [MaxLength(30)]
        public string? Phone { get; set; }
        [EmailAddress, MaxLength(254)]
        public string? Email { get; set; }
        [MaxLength(250)]
        public string? AddressLine1 { get; set; }
        [MaxLength(250)]
        public string? AddressLine2 { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }
        [MaxLength(150)]
        public string? EmergencyContactName { get; set; }
        [MaxLength(30)]
        public string? EmergencyContactPhone { get; set; }
        public bool CreateAccount { get; set; }
        public string? Password { get; set; }
    }
}
