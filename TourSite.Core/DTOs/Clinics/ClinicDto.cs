
namespace TourSite.Core.DTOs.Clinics
{
    public class ClinicDto
    {
        [System.ComponentModel.DataAnnotations.MaxLength(150)]
        public string? ClinicNameAr { get; set; }
        [System.ComponentModel.DataAnnotations.MaxLength(500)]
        public string? WorkingHoursNotesAr { get; set; }
        public int Id { get; set; }
        public string DisplayName => System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "ar" && !string.IsNullOrWhiteSpace(ClinicNameAr) ? ClinicNameAr : ClinicName;
        public string ClinicCode { get; set; } = string.Empty;
        public string ClinicName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? WorkingHoursNotes { get; set; }
        public string? ImageCover { get; set; }
        public bool IsActive { get; set; }
    }
}
