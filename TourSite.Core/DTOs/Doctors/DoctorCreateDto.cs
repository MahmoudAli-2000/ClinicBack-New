using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorCreateDto
    {
        [Required]
        public int StaffId { get; set; }
        [Required]
        public int SpecialtyId { get; set; }
        [Required, MaxLength(80)]
        public string LicenseNo { get; set; } = string.Empty;
        [MaxLength(500)]
        public string? ImageCover { get; set; }
        public IFormFile? ImageFile { get; set; }
        public short ConsultationDurationMinutes { get; set; } = 30;
        public decimal? DefaultConsultationFee { get; set; }
        [MaxLength(1500)]
        public string? Bio { get; set; }
    }
}
