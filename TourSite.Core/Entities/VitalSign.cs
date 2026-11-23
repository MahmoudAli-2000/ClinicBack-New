using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    public class VitalSign
    {
        [Key]
        public int Id { get; set; }

        public int VisitId { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? WeightKg { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal? HeightCm { get; set; }

        public short? SystolicBP { get; set; }
        public short? DiastolicBP { get; set; }
        public short? HeartRateBpm { get; set; }

        [Column(TypeName = "decimal(4,1)")]
        public decimal? TemperatureC { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal? OxygenSaturationPct { get; set; }

        public short? RespiratoryRate { get; set; }
        public int? RecordedByStaffId { get; set; }
        public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Relations
        [ForeignKey(nameof(VisitId))]
        public Visit Visit { get; set; } = null!;

        [ForeignKey(nameof(RecordedByStaffId))]
        public Staff? RecordedByStaff { get; set; }
    }
}
