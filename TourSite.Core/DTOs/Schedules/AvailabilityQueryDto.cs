using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Schedules
{
    public class AvailabilityQueryDto
    {
        [Required]
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
        [Required]
        public DateTime DateUtc { get; set; }
    }
}
