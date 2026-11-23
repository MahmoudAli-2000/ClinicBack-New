using TourSite.Core.Utilities.Enums;

namespace TourSite.Core.DTOs.Staff
{
    public class StaffDto
    {
        public int Id { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ImageCover { get; set; }
        public string PublicCode { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? PrimaryClinicId { get; set; }
        public string? JobTitle { get; set; }
        public StaffStatus Status { get; set; }
        public DateTime? HireDate { get; set; }
        public int? DoctorId { get; set; }
        public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
    }
}
