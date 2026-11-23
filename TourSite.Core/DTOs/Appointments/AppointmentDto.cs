using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Appointments
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int DoctorClinicId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime ScheduledStartAtUtc { get; set; }
        public DateTime ScheduledEndAtUtc { get; set; }
        public AppointmentStatus Status { get; set; }
        public BookingSource BookingSource { get; set; }
        public string? Reason { get; set; }
        public string? Notes { get; set; }
        public int? VisitId { get; set; }
        public bool HasReview { get; set; }
    }
}
