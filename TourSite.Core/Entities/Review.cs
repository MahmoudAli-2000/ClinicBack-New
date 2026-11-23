using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    [Index(nameof(AppointmentId), IsUnique = true)]
    public class Review
    {
        [Key]
        public int Id { get; set; }

        public int AppointmentId { get; set; }

        public byte? DoctorRating { get; set; }
        public byte? ClinicRating { get; set; }
        public byte? StaffRating { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        public bool IsAnonymous { get; set; } = false;
        public DateTime ReviewedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;
    }
}
