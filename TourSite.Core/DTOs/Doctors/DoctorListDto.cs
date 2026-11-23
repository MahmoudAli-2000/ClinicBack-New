
namespace TourSite.Core.DTOs.Doctors
{
    public class DoctorListDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; } = string.Empty;
        public string? ImageCover { get; set; }
        public string PublicCode { get; set; } = string.Empty;
        public string LicenseNo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public short ConsultationDurationMinutes { get; set; }
    }
}
