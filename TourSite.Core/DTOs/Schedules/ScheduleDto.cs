
namespace TourSite.Core.DTOs.Schedules
{
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int DoctorClinicId { get; set; }
        public byte DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public short SlotDurationMinutes { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}
