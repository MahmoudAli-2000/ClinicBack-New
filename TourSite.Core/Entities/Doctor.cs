using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    [Index(nameof(StaffId), IsUnique = true)]
    [Index(nameof(LicenseNo), IsUnique = true)]
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PublicCode { get; set; } = string.Empty;

        public int StaffId { get; set; }
        public int SpecialtyId { get; set; }

        [Required, MaxLength(80)]
        public string LicenseNo { get; set; } = string.Empty;

        public short ConsultationDurationMinutes { get; set; } = 30;

        [Column(TypeName = "decimal(12,2)")]
        public decimal? DefaultConsultationFee { get; set; }

        [MaxLength(1500)]
        public string? Bio { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(StaffId))]
        public Staff Staff { get; set; } = null!;

        [ForeignKey(nameof(SpecialtyId))]
        public Specialty Specialty { get; set; } = null!;

        public ICollection<DoctorClinic> DoctorClinics { get; set; } = new List<DoctorClinic>();
        public ICollection<DoctorTimeOff> TimeOffs { get; set; } = new List<DoctorTimeOff>();
        public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
