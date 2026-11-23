using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Schedules
{
    public class ScheduleCreateDto
    {
        [Required]
        public int DoctorClinicId { get; set; }
        [Range(1, 7)]
        public byte DayOfWeek { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        public short SlotDurationMinutes { get; set; } = 30;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
