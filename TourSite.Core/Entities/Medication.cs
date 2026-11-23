using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.Entities
{
    [Index(nameof(MedicationCode), IsUnique = true)]
    public class Medication
    {
        [MaxLength(200)]
        public string? GenericNameAr { get; set; }

        [MaxLength(200)]
        public string? BrandNameAr { get; set; }

        [MaxLength(100)]
        public string? DosageFormAr { get; set; }

        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string? MedicationCode { get; set; }

        [MaxLength(200)]
        public string? BrandName { get; set; }

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string GenericName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DosageForm { get; set; }

        [MaxLength(100)]
        public string? Strength { get; set; }

        [MaxLength(50)]
        public string? Unit { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<PrescriptionItem> PrescriptionItems { get; set; } = new List<PrescriptionItem>();
    }
}
