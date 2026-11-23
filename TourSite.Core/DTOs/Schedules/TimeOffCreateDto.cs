using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Schedules
{
    public class TimeOffCreateDto
    {
        [Required]
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
        [Required]
        public DateTime StartAtUtc { get; set; }
        [Required]
        public DateTime EndAtUtc { get; set; }
        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
