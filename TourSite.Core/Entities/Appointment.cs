using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public int PatientId { get; set; }
        public int DoctorClinicId { get; set; }

        public DateTime ScheduledStartAtUtc { get; set; }

        public DateTime ScheduledEndAtUtc { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public BookingSource BookingSource { get; set; } = BookingSource.Admin;

        [MaxLength(1000)]
        public string? Reason { get; set; }

        [MaxLength(1500)]
        public string? Notes { get; set; }

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime? CancelledAtUtc { get; set; }

        public DateTime? ReminderSentAtUtc { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;

        [ForeignKey(nameof(DoctorClinicId))]
        public DoctorClinic DoctorClinic { get; set; } = null!;

        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedBy { get; set; }

        public Visit? Visit { get; set; }
        public Review? Review { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
