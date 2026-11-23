using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.Entities
{
    public class Diagnosis
    {
        [Key]
        public int Id { get; set; }

        public int VisitId { get; set; }

        [MaxLength(30)]
        public string? DiagnosisCode { get; set; }

        [Required, MaxLength(250)]
        public string DiagnosisName { get; set; } = string.Empty;

        [Required, MaxLength(20)]
        public DiagnosisType DiagnosisType { get; set; }

        [MaxLength(1500)]
        public string? Notes { get; set; }

        public int? DiagnosedByDoctorId { get; set; }
        public DateTime DiagnosedAtUtc { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(VisitId))]
        public Visit Visit { get; set; } = null!;

        [ForeignKey(nameof(DiagnosedByDoctorId))]
        public Doctor? DiagnosedByDoctor { get; set; }
    }
}
