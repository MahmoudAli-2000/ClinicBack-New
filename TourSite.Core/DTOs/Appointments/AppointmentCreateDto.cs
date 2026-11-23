using System.ComponentModel.DataAnnotations;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Appointments
{
    public class AppointmentCreateDto
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorClinicId { get; set; }
        [Required]
        public DateTime ScheduledStartAtUtc { get; set; }
        public DateTime? ScheduledEndAtUtc { get; set; }
        public BookingSource BookingSource { get; set; } = BookingSource.Admin;
        [MaxLength(1000)]
        public string? Reason { get; set; }
        [MaxLength(1500)]
        public string? Notes { get; set; }
    }
}
