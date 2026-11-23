
namespace TourSite.Core.DTOs.Prescriptions
{
    public class PrescriptionItemDto
    {
        public int Id { get; set; }
        public int MedicationId { get; set; }
        public string MedicationName { get; set; } = string.Empty;
        public string? Dose { get; set; }
        public string? Frequency { get; set; }
        public short? DurationDays { get; set; }
        public string? Route { get; set; }
        public decimal? Quantity { get; set; }
        public string? Instructions { get; set; }
    }
}
