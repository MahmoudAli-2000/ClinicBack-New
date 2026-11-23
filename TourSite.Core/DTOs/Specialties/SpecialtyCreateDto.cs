using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Specialties
{
    public class SpecialtyCreateDto
    {
        [Required, MaxLength(120)]
        public string SpecialtyName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
