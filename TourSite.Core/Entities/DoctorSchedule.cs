using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    public class DoctorSchedule
    {
        [Key]
        public int Id { get; set; }

        public int DoctorClinicId { get; set; }

        // 1 = Monday ... 7 = Sunday
        public byte DayOfWeek { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public short SlotDurationMinutes { get; set; } = 30;

        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(DoctorClinicId))]
        public DoctorClinic DoctorClinic { get; set; } = null!;
    }
}
