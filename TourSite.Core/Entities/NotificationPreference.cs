using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    [Index(nameof(UserId), IsUnique = true)]
    public class NotificationPreference
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(450)]
        public string UserId { get; set; } = string.Empty;

        public bool AllowInApp { get; set; } = true;
        public bool AllowEmail { get; set; } = true;
        public bool AllowSMS { get; set; } = true;
        public bool AllowPush { get; set; } = true;
        public bool AllowWhatsApp { get; set; } = false;

        public bool AppointmentUpdates { get; set; } = true;
        public bool AppointmentReminders { get; set; } = true;
        public bool GeneralUpdates { get; set; } = true;

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;
    }
}
