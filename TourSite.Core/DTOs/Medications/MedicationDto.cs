
namespace TourSite.Core.DTOs.Medications
{
    public class MedicationDto
    {
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? GenericNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? BrandNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string? DosageFormAr { get; set; }
        public int Id { get; set; }
        public string DisplayName => System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" && !string.IsNullOrWhiteSpace(GenericNameAr) ? GenericNameAr : GenericName;
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
