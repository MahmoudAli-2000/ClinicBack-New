using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Specialties
{
    public class SpecialtyCreateDto
    {
        [System.ComponentModel.DataAnnotations.MaxLength(120)]
        public string? SpecialtyNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public string? DescriptionAr { get; set; }
        [Required, MaxLength(120)]
        public string SpecialtyName { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? Description { get; set; }
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}
