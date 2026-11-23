using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Schedules
{
    public class TimeOffDto
    {
        public int Id { get; set; }
        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }
        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }
        public string? Reason { get; set; }
        public TimeOffStatus Status { get; set; }
    }
}
