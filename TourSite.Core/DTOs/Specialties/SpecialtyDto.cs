
namespace TourSite.Core.DTOs.Specialties
{
    public class SpecialtyDto
    {
        [System.ComponentModel.DataAnnotations.MaxLength(120)]
        public string? SpecialtyNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public string? DescriptionAr { get; set; }
        public int Id { get; set; }
        public string DisplayName => System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" && !string.IsNullOrWhiteSpace(SpecialtyNameAr) ? SpecialtyNameAr : SpecialtyName;
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageCover { get; set; }
        public bool IsActive { get; set; }
    }
}
