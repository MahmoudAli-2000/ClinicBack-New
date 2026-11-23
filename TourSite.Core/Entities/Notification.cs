using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        public int? AppointmentId { get; set; }

        [Required, MaxLength(50)]
        public string NotificationType { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public NotificationChannel Channel { get; set; } = NotificationChannel.InApp;

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Message { get; set; } = string.Empty;

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;

        public DateTime? ScheduledAtUtc { get; set; }
        public DateTime? SentAtUtc { get; set; }
        public DateTime? ReadAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }
    }
}
