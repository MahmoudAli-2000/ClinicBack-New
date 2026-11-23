using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.Entities
{
    [Index(nameof(SpecialtyName), IsUnique = true)]
    public class Specialty
    {
        [MaxLength(120)]
        public string? SpecialtyNameAr { get; set; }

        [MaxLength(500)]
        public string? DescriptionAr { get; set; }

        [Key]
        public int Id { get; set; }

        [Required, MaxLength(120)]
        public string SpecialtyName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(500)]
        public string ImageCover { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
    }
}
