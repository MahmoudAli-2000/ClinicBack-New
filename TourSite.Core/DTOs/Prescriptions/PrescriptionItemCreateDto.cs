using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Prescriptions
{
    public class PrescriptionItemCreateDto
    {
        [Required]
        public int MedicationId { get; set; }
        [MaxLength(100)]
        public string? Dose { get; set; }
        [MaxLength(150)]
        public string? Frequency { get; set; }
        public short? DurationDays { get; set; }
        [MaxLength(100)]
        public string? Route { get; set; }
        public decimal? Quantity { get; set; }
        [MaxLength(1000)]
        public string? Instructions { get; set; }
    }
}
