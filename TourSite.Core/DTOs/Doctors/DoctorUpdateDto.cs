using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorUpdateDto
    {
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
        public bool IsActive { get; set; } = true;
    }
}
