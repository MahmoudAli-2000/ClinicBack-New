using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Appointments
{
    public class AppointmentRescheduleDto
    {
        [Required]
        public DateTime ScheduledStartAtUtc { get; set; }
        public DateTime? ScheduledEndAtUtc { get; set; }
        public int? DoctorClinicId { get; set; }
        [MaxLength(1500)]
        public string? Notes { get; set; }
    }
}
