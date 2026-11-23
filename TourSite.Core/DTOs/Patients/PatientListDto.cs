using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Patients
{
    public class PatientListDto
    {
        public int Id { get; set; }
        public string PatientNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ImageCover { get; set; }
        public string PublicCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public PatientStatus Status { get; set; }
    }
}
