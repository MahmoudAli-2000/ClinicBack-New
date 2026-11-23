using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourSite.Core.Entities
{
    public class PrescriptionItem
    {
        [Key]
        public int Id { get; set; }

        public int PrescriptionId { get; set; }
        public int MedicationId { get; set; }

        [MaxLength(100)]
        public string? Dose { get; set; }

        [MaxLength(150)]
        public string? Frequency { get; set; }

        public short? DurationDays { get; set; }

        [MaxLength(100)]
        public string? Route { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Quantity { get; set; }

        [MaxLength(1000)]
        public string? Instructions { get; set; }

        // Relations
        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; } = null!;

        [ForeignKey(nameof(MedicationId))]
        public Medication Medication { get; set; } = null!;
    }
}
