using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        public int VisitId { get; set; }
        public int PrescribedByDoctorId { get; set; }

        
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Active;

        public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(1500)]
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(VisitId))]
        public Visit Visit { get; set; } = null!;

        [ForeignKey(nameof(PrescribedByDoctorId))]
        public Doctor PrescribedByDoctor { get; set; } = null!;

        public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
    }
}
