using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Medications
{
    public class MedicationCreateDto
    {
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? GenericNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? BrandNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string? DosageFormAr { get; set; }
        [MaxLength(50)]
        public string? MedicationCode { get; set; }
        [MaxLength(200)]
        public string? BrandName { get; set; }
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
        [Required, MaxLength(200)]
        public string GenericName { get; set; } = string.Empty;
        [MaxLength(100)]
        public string? DosageForm { get; set; }
        [MaxLength(100)]
        public string? Strength { get; set; }
        [MaxLength(50)]
        public string? Unit { get; set; }
    }
}
