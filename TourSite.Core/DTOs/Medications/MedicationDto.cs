
namespace TourSite.Core.DTOs.Medications
{
    public class MedicationDto
    {
        public int Id { get; set; }
        public string? MedicationCode { get; set; }
        public string? BrandName { get; set; }
        public string? ImageCover { get; set; }
        public string GenericName { get; set; } = string.Empty;
        public string? DosageForm { get; set; }
        public string? Strength { get; set; }
        public string? Unit { get; set; }
        public bool IsActive { get; set; }
    }
}
