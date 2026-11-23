using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Clinics
{
    public class ClinicCreateDto
    {
        [Required, MaxLength(30)]
        public string ClinicCode { get; set; } = string.Empty;
        [Required, MaxLength(150)]
        public string ClinicName { get; set; } = string.Empty;
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
        [MaxLength(100)]
        public string? Country { get; set; }
        [MaxLength(500)]
        public string? WorkingHoursNotes { get; set; }
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
