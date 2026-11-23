using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    public class DoctorTimeOff
    {
        [Key]
        public int Id { get; set; }

        public int DoctorId { get; set; }
        public int? ClinicId { get; set; }

        public DateTime StartAtUtc { get; set; }
        public DateTime EndAtUtc { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public TimeOffStatus Status { get; set; } = TimeOffStatus.Approved;

        [MaxLength(450)]
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(ClinicId))]
        public Clinic? Clinic { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public User? CreatedBy { get; set; }
    }
}
