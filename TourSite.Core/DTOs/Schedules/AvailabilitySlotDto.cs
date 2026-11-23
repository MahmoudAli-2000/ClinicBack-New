
namespace TourSite.Core.DTOs.Schedules
{
    public class AvailabilitySlotDto
    {
        public int DoctorClinicId { get; set; }
        public int ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }
    }
}
