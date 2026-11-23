using System.ComponentModel.DataAnnotations;

namespace TourSite.Core.DTOs.Prescriptions
{
    public class PrescriptionCreateDto
    {
        [Required]
        public int VisitId { get; set; }
        [MaxLength(1500)]
        public string? Notes { get; set; }
        [Required, MinLength(1)]
        public List<PrescriptionItemCreateDto> Items { get; set; } = new();
    }
}
