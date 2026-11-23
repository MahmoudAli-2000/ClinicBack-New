using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.Entities
{
    [Index(nameof(ClinicCode), IsUnique = true)]
    public class Clinic
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string ClinicCode { get; set; } = string.Empty;

        [Required, MaxLength(150)]
        public string ClinicName { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(254)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        [MaxLength(250)]
        public string? AddressLine1 { get; set; }

        [MaxLength(250)]
        public string? AddressLine2 { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(500)]
        public string? WorkingHoursNotes { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<Staff> StaffMembers { get; set; } = new List<Staff>();
        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
        public ICollection<DoctorTimeOff> DoctorTimeOffs { get; set; } = new List<DoctorTimeOff>();
    }
}
