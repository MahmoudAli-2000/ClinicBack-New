using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    [Index(nameof(EmployeeNo), IsUnique = true)]
    [Index(nameof(UserId), IsUnique = true)]
    public class Staff
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        [Required, MaxLength(30)]
        public string EmployeeNo { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(254)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PublicCode { get; set; } = string.Empty;

        public int? PrimaryClinicId { get; set; }

        [MaxLength(120)]
        public string? JobTitle { get; set; }

        [Required, MaxLength(20)]
        public StaffStatus Status { get; set; } 

        public DateTime? HireDate { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(PrimaryClinicId))]
        public Clinic? PrimaryClinic { get; set; }

        public Doctor? Doctor { get; set; }
        public ICollection<VitalSign> RecordedVitalSigns { get; set; } = new List<VitalSign>();
    }
}
