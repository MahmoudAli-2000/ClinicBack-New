using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.Entities
{
    public class User : IdentityUser
    {
        public string FullName { get; set; }
        public bool IsActive { get; set; } = true;

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        // One-to-one business profiles
        public Patient? Patient { get; set; }
        public Staff? Staff { get; set; }

        // Relations
        public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorTimeOff> CreatedDoctorTimeOffs { get; set; } = new List<DoctorTimeOff>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public NotificationPreference? NotificationPreference { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
